using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Application.EnseignantsPrincipaux.GetAllEnseignantsPrincipaux;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Application.Inscriptions.AddPresencesForClasse;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Classes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("Classes")]
    public class ClasseController : Controller
    {
        private readonly IMediator _mediator;

        public ClasseController(IMediator mediator)
        {
            _mediator = mediator;
        }

       

        // GET: /Classes/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            // Appel de la query pour obtenir les enseignants principaux
            var result = await _mediator.Send(new GetAllEnseignantsPrincipauxQuery());

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                ViewBag.Enseignants = new List<SelectListItem>();
                return View();
            }

            // Conversion en SelectList pour la vue (DropDownList)
            var enseignants = result.Value
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"{e.Prenom} {e.Nom}"
                })
                .ToList();

            ViewBag.Enseignants = enseignants;

            return View();
        }

        // POST: /Classes/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClasseDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var cmd = new CreateClasseCommand(
                dto.NomClasse,
                dto.AnneeScolaire,
                dto.IdEnseignantPrincipal
            );

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                // TODO: recharger dropdown si nécessaire
                return View(dto);
            }

            TempData["SuccessMessage"] = "Classe créée avec succès !";
            return RedirectToAction("Index", "Home");
        }
        // GET: /Classe/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            // 1) Récupérer la classe
            var classeResult = await _mediator.Send(new GetClasseByIdQuery(id));
            if (!classeResult.IsSuccess)
                return NotFound(classeResult.Error);

            // 2) Récupérer les inscriptions
            var inscResult = await _mediator.Send(new GetInscriptionsByClasseQuery(id));
            if (!inscResult.IsSuccess)
                ModelState.AddModelError(string.Empty, inscResult.Error);

            var vm = new ClasseDetailsViewModel
            {
                ClassId = id,
                NomClasse = classeResult.Value.NomClasse,
                Inscriptions = inscResult.IsSuccess
                    ? inscResult.Value
                    : new List<GetInscriptionsByClasseDto>()
            };

            return View(vm);
        }

        [HttpGet("{classeId:guid}/Inscription/Create")]
        public IActionResult CreateInscription(Guid classeId)
        {

            var cmd = new AddInscriptionCommand(
                ClasseId: classeId,
                NoImmatricule: string.Empty,
                DateDebut: DateTime.Today,
                DateFin: null
            );
            return View(cmd);
        }

        // POST: /Inscription/Create
        [HttpPost("{classeId:guid}/Inscription/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInscription(AddInscriptionCommand cmd)
        {
            if (!ModelState.IsValid)
                return View(cmd);


            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(cmd);
            }

            TempData["SuccessMessage"] = "Inscription ajoutée avec succès !";

            return RedirectToAction("Details", "Classe", new { id = cmd.ClasseId });
        }

        // GET: /Classes/{id}/AddNotes
        [HttpGet("{classeId:guid}/AddNotes")]
        public async Task<IActionResult> AddNotes(Guid classeId)
        {
           
            // 1) inscriptions
            var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
            if (!inscR.IsSuccess)
                return NotFound(inscR.Error);

            // 2) matières
            var matR = await _mediator.Send(new GetAllMatieresQuery());
            var matieres = matR.IsSuccess
              ? matR.Value.Select(m => new SelectListItem(m.Nom, m.Id.ToString())).ToList()
              : new List<SelectListItem>();

            // 3) construire le VM
            var vm = new NotesForClasseViewModel
            {
                ClasseId = classeId,
                DateExamen = DateTime.Today,
                Matieres = matieres,
                Notes = inscR.Value.Select(i => new EleveNoteEntry
                {
                    EleveId = i.EleveId,
                    EleveName = i.EleveNom,      
                    Valeur = -1,
                    Commentaire = null
                }).ToList()
            };
            return View(vm);
        }

        // POST: /Classes/AddNotes
        [HttpPost("{classeId:guid}/AddNotes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotes(NotesForClasseViewModel vm, Guid classeId)
        {
            vm.ClasseId = classeId;
            if (!ModelState.IsValid)
            {
                // recharger matières
                var matR = await _mediator.Send(new GetAllMatieresQuery());
                vm.Matieres = matR.IsSuccess
                  ? matR.Value.Select(m => new SelectListItem(m.Nom, m.Id.ToString())).ToList()
                  : new List<SelectListItem>();               
                return View(vm);
            }

            // Injecte la matière choisie dans chaque note
            var notesDto = vm.Notes
                .Select(n => new NoteForEleveDto(
                    EleveId: n.EleveId,
                    MatiereId: vm.SelectedMatiereId,
                    Valeur: n.Valeur==0? (double?)null : n.Valeur,
                    Commentaire: n.Commentaire))
                .ToList();

            var cmd = new AddNotesForClasseCommand(vm.ClasseId, vm.DateExamen, notesDto);
            var result = await _mediator.Send(cmd);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                var matR = await _mediator.Send(new GetAllMatieresQuery());
                vm.Matieres = matR.IsSuccess
                  ? matR.Value.Select(m => new SelectListItem(m.Nom, m.Id.ToString())).ToList()
                  : new List<SelectListItem>();
                //recharger les inscriptions pour retrouver EleveName
                var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
                if (inscR.IsSuccess)
                {
                    vm.Notes = inscR.Value.Select(i =>
                    {
                        var posted = vm.Notes.FirstOrDefault(n => n.EleveId == i.EleveId);
                        return new EleveNoteEntry
                        {
                            EleveId = i.EleveId,
                            EleveName = i.EleveNom,
                            Valeur = posted?.Valeur ?? -1,
                            Commentaire = posted?.Commentaire
                        };
                    }).ToList();
                }
                return View(vm);
            }

            TempData["SuccessMessage"] = $"{result.Value} notes enregistrées.";
            return RedirectToAction("Details", new { id = vm.ClasseId });
        }
        // GET: /Classes/{classeId}/AddPresences
        [HttpGet("{classeId:guid}/AddPresences")]
        public async Task<IActionResult> AddPresences(Guid classeId)
        {
            var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
            if (!inscR.IsSuccess)
                return NotFound(inscR.Error);

            var vm = new PresencesForClasseViewModel
            {
                ClasseId = classeId,
                Date = DateTime.Today,
                Periode = 1,  
                Presences = inscR.Value
                    .Select(i => new ElevePresenceEntry
                    {
                        EleveId = i.EleveId,
                        EleveName = i.EleveNom,
                        Statut = ""  
                    })
                    .ToList()
            };
            return View(vm);
        }

        // POST: /Classes/{classeId}/AddPresences
        [HttpPost("{classeId:guid}/AddPresences")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPresences(
            Guid classeId,
            PresencesForClasseViewModel vm)
        {
            vm.ClasseId = classeId;

            if (!ModelState.IsValid)
            {
                var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
                if (inscR.IsSuccess)
                {
                    vm.Presences = inscR.Value.Select(i => new ElevePresenceEntry
                    {
                        EleveId = i.EleveId,
                        EleveName = i.EleveNom,
                        Statut = vm.Presences.FirstOrDefault(p => p.EleveId == i.EleveId)?.Statut ?? ""
                    }).ToList();
                }
               
                return View(vm);
            }

            // Transformer vers DTO
            var presencesDto = vm.Presences
                .Select(p => new PresenceEleveDto(
                    EleveId: p.EleveId,
                    Statut: p.Statut))
                .ToList();

            var cmd = new AddPresencesForClasseCommand(
                ClasseId: classeId,
                Date: vm.Date,
                Periode: vm.Periode,
                Presences: presencesDto);

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
                if (inscR.IsSuccess)
                {
                    vm.Presences = inscR.Value.Select(i => new ElevePresenceEntry
                    {
                        EleveId = i.EleveId,
                        EleveName = i.EleveNom,
                        Statut = vm.Presences.FirstOrDefault(p => p.EleveId == i.EleveId)?.Statut ?? ""
                    }).ToList();
                }
                return View(vm);
            }

            TempData["SuccessMessage"] = $"{result.Value} présences enregistrées.";
            return RedirectToAction("Details", new { id = classeId });
        }

        // POST: /Classes/{classeId}/Inscription/Delete/{eleveId}
        [HttpPost("{classeId:guid}/Inscription/Delete/{eleveId:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInscription(Guid classeId, Guid eleveId)
        {
            var cmd = new RemoveInscriptionCommand(classeId, eleveId);
            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Inscription supprimée avec succès.";
            }
            return RedirectToAction("Details", new { id = classeId });
        }
    }
}
