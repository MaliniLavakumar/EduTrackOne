using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Application.Eleves.GetEleveById;
using EduTrackOne.Application.EnseignantsPrincipaux.GetAllEnseignantsPrincipaux;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Application.Inscriptions.AddPresencesForClasse;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Classes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("Classes")]
    public class ClasseController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClasseController> _logger;

        public ClasseController(IMediator mediator, ILogger<ClasseController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        // GET: /Classes/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("GET Create Classe - début");
            // Appel de la query pour obtenir les enseignants principaux
            var result = await _mediator.Send(new GetAllEnseignantsPrincipauxQuery());

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Échec GetAllEnseignantsPrincipauxQuery: {Error}", result.Error);
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
            _logger.LogInformation("GET Create Classe - fin, {Count} enseignants récupérés", enseignants.Count);

            return View();
        }
        [Authorize(Roles = "Admin")]
        // POST: /Classes/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClasseDto dto)
        {
            _logger.LogInformation("POST Create Classe - début, NomClasse={NomClasse}, AnneeScolaire={Annee}, EnseignantId={EnsId}",
       dto.NomClasse, dto.AnneeScolaire, dto.IdEnseignantPrincipal);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST Create Classe - ModelState invalide");
                return View(dto);
            }


            var cmd = new CreateClasseCommand(
                dto.NomClasse,
                dto.AnneeScolaire,
                dto.IdEnseignantPrincipal
            );

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("CreateClasseCommand échoué: {Error}", result.Error);
                ModelState.AddModelError(string.Empty, result.Error);
                return View(dto);
            }

            _logger.LogInformation("Classe créée avec succès, Id={ClasseId}", result.Value);
            TempData["SuccessMessage"] = "Classe créée avec succès !";
            return RedirectToAction("Index", "Home");
        }
        // GET: /Classe/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation("GET Details Classe - début pour Id={Id}", id);

            // 1) Récupérer la classe
            var classeResult = await _mediator.Send(new GetClasseByIdQuery(id));
            if (!classeResult.IsSuccess)
            {
                _logger.LogWarning("GetClasseByIdQuery échoué pour Id={Id}: {Error}", id, classeResult.Error);
                return NotFound(classeResult.Error);
            }

            // 2) Récupérer les inscriptions
            var inscResult = await _mediator.Send(new GetInscriptionsByClasseQuery(id));
            if (!inscResult.IsSuccess)
            {
                _logger.LogWarning("GetInscriptionsByClasseQuery échoué pour ClasseId={Id}: {Error}", id, inscResult.Error);
                ModelState.AddModelError(string.Empty, inscResult.Error);
            }
                

            var vm = new ClasseDetailsViewModel
            {
                ClassId = id,
                NomClasse = classeResult.Value.NomClasse,
                Inscriptions = inscResult.IsSuccess
                    ? inscResult.Value
                    : new List<GetInscriptionsByClasseDto>()
            };
            _logger.LogInformation("Get Deatails Classe - fin pour Id= {Id}, InscriptionsCount = {Count}", id, vm.Inscriptions.Count());

            return View(vm);
        }
        [Authorize(Roles = "Enseignant")]
        [HttpGet("{classeId:guid}/Inscription/Create")]
        public async Task<IActionResult> CreateInscription(Guid classeId)
        {
            _logger.LogInformation("GET CreateInscription - début pour ClasseId={ClasseId}", classeId);

            // 1) Récupérer le nom de la classe
            var classeResult = await _mediator.Send(new GetClasseByIdQuery(classeId));
            if (!classeResult.IsSuccess)
            {
                _logger.LogWarning("GetClasseByIdQuery échoué dans CreateInscription pour ClasseId={ClasseId}: {Error}", classeId, classeResult.Error);
                TempData["ErrorMessage"] = classeResult.Error;
                return RedirectToAction("Index", "Classe"); 
            }

            // 2) Remplir le ViewModel
            var vm = new CreateInscriptionViewModel
            {
                ClasseId = classeId,
                ClasseName = classeResult.Value.NomClasse,
                DateDebut = DateTime.Today,
                DateFin = null,
                NoImmatricule = string.Empty
            };
            _logger.LogInformation("GET CreateInscription - fin pour ClasseId={ClasseId}", classeId);
            return View(vm);
        }

        [Authorize(Roles = "Enseignant")]
        // POST: /Inscription/Create
        [HttpPost("{classeId:guid}/Inscription/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInscription(CreateInscriptionViewModel vm)
        {
            _logger.LogInformation("POST CreateInscription - début pour ClasseId={ClasseId}, NoImmatricule={Immatricule}",
        vm.ClasseId, vm.NoImmatricule);
            // 1) Vérifier ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST CreateInscription - ModelState invalide pour ClasseId={ClasseId}", vm.ClasseId);
                if (string.IsNullOrEmpty(vm.ClasseName))
                {
                    var classeResult = await _mediator.Send(new GetClasseByIdQuery(vm.ClasseId));
                    if (classeResult.IsSuccess)
                        vm.ClasseName = classeResult.Value.NomClasse;
                }
                return View(vm);
            }

            // 2) Construire la commande MediatR
            var cmd = new AddInscriptionCommand(
                ClasseId: vm.ClasseId,
                NoImmatricule: vm.NoImmatricule,
                DateDebut: vm.DateDebut,
                DateFin: vm.DateFin
            );

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("AddInscriptionCommand échoué pour ClasseId={ClasseId}, EleveImmatricule={Immatricule}: {Error}",
                vm.ClasseId, vm.NoImmatricule, result.Error);

                // Ajouter l’erreur métier
                ModelState.AddModelError(string.Empty, result.Error);
                // Recharger ClasseName pour l’affichage
                if (string.IsNullOrEmpty(vm.ClasseName))
                {
                    var classeResult = await _mediator.Send(new GetClasseByIdQuery(vm.ClasseId));
                    if (classeResult.IsSuccess)
                        vm.ClasseName = classeResult.Value.NomClasse;
                }
                return View(vm);
            }
            _logger.LogInformation("Inscription ajoutée avec succès pour ClasseId={ClasseId}, EleveImmatricule={Immatricule}",
        vm.ClasseId, vm.NoImmatricule);
            TempData["SuccessMessage"] = "Inscription ajoutée avec succès !";
            return RedirectToAction("Details", new { id = vm.ClasseId });
        }


        // GET: /Classes/{id}/AddNotes
        [HttpGet("{classeId:guid}/AddNotes")]
        public async Task<IActionResult> AddNotes(Guid classeId)
        {
            _logger.LogInformation("GET AddNotes - début pour ClasseId={ClasseId}", classeId);
            // 1) inscriptions
            var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
            if (!inscR.IsSuccess)
            {
                _logger.LogWarning("GetInscriptionsByClasseQuery échoué dans AddNotes pour ClasseId={ClasseId}: {Error}", classeId, inscR.Error);
                return NotFound(inscR.Error);
            }
               

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
            _logger.LogInformation("GET AddNotes - fin pour ClasseId={ClasseId}, ElevesCount={Count}, MatieresCount={MatCount}",
        classeId, vm.Notes.Count, vm.Matieres.Count);
            return View(vm);
        }

        // POST: /Classes/AddNotes
        [HttpPost("{classeId:guid}/AddNotes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotes(NotesForClasseViewModel vm, Guid classeId)
        {
            _logger.LogInformation("POST AddNotes - début pour ClasseId={ClasseId}, MatiereId={MatiereId}, EntriesCount={Count}",
       classeId, vm.SelectedMatiereId, vm.Notes?.Count ?? 0);

            vm.ClasseId = classeId;
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST AddNotes - ModelState invalide pour ClasseId={ClasseId}", classeId);
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
                    Valeur: n.Valeur == 0 ? (double?)null : n.Valeur,
                    Commentaire: n.Commentaire))
                .ToList();

            var cmd = new AddNotesForClasseCommand(vm.ClasseId, vm.DateExamen, notesDto);
            var result = await _mediator.Send(cmd);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("AddNotesForClasseCommand échoué pour ClasseId={ClasseId}: {Error}", classeId, result.Error);
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
            _logger.LogInformation("{Count} notes enregistrées pour ClasseId={ClasseId}", result.Value, classeId);
            TempData["SuccessMessage"] = $"{result.Value} notes enregistrées.";
            return RedirectToAction("Details", new { id = vm.ClasseId });
        }
        // GET: /Classes/{classeId}/AddPresences
        [HttpGet("{classeId:guid}/AddPresences")]
        public async Task<IActionResult> AddPresences(Guid classeId)
        {
            _logger.LogInformation("GET AddPresences - début pour ClasseId={ClasseId}", classeId);
            var inscR = await _mediator.Send(new GetInscriptionsByClasseQuery(classeId));
            if (!inscR.IsSuccess)
            {
                _logger.LogWarning("GetInscriptionsByClasseQuery échoué dans AddPresences pour ClasseId={ClasseId}: {Error}", classeId, inscR.Error);
                return NotFound(inscR.Error);
            }
               

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
            _logger.LogInformation("GET AddPresences - fin pour ClasseId={ClasseId}, ElevesCount={Count}", classeId, vm.Presences.Count);
            return View(vm);
        }

        // POST: /Classes/{classeId}/AddPresences
        [HttpPost("{classeId:guid}/AddPresences")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPresences(
            Guid classeId,
            PresencesForClasseViewModel vm)
        {
            _logger.LogInformation("POST AddPresences - début pour ClasseId={ClasseId}, EntriesCount={Count}", classeId, vm.Presences?.Count ?? 0);
            vm.ClasseId = classeId;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST AddPresences - ModelState invalide pour ClasseId={ClasseId}", classeId);
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
                _logger.LogWarning("AddPresencesForClasseCommand échoué pour ClasseId={ClasseId}: {Error}", classeId, result.Error);
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
            _logger.LogInformation("{Count} présences enregistrées pour ClasseId={ClasseId}", result.Value, classeId);
            TempData["SuccessMessage"] = $"{result.Value} présences enregistrées.";
            return RedirectToAction("Details", new { id = classeId });
        }

        // GET: /Classes/{classeId}/Inscription/ConfirmDelete/{eleveId}
        [HttpGet("{classeId:guid}/Inscription/ConfirmDelete/{eleveId:guid}")]
        public async Task<IActionResult> ConfirmDeleteInscription(Guid classeId, Guid eleveId)
        {
            _logger.LogInformation("GET ConfirmDeleteInscription - ClasseId={ClasseId}, EleveId={EleveId}", classeId, eleveId);
            var eleveResult = await _mediator.Send(new GetEleveByIdQuery(eleveId));
            if (!eleveResult.IsSuccess)
            {
                _logger.LogWarning("GetEleveByIdQuery échoué pour EleveId={EleveId}", eleveId);
                return NotFound();
            }
               
            ViewData["ClasseId"] = classeId;
            return View("ConfirmDeleteInscription", eleveResult.Value); // Passe un EleveDto à la vue
        }


        // POST: /Classes/{classeId}/Inscription/Delete/{eleveId}
        [HttpPost("{classeId:guid}/Inscription/Delete/{eleveId:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInscription(Guid classeId, Guid eleveId)
        {
            _logger.LogInformation("POST DeleteInscription - début pour ClasseId={ClasseId}, EleveId={EleveId}", classeId, eleveId);
            var cmd = new RemoveInscriptionCommand(classeId, eleveId);
            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("RemoveInscriptionCommand échoué pour ClasseId={ClasseId}, EleveId={EleveId}: {Error}", classeId, eleveId, result.Error);
                TempData["ErrorMessage"] = result.Error;
            }
            else
            {
                _logger.LogInformation("Inscription supprimée avec succès pour ClasseId={ClasseId}, EleveId={EleveId}", classeId, eleveId);
                TempData["SuccessMessage"] = "Inscription supprimée avec succès.";
            }
            return RedirectToAction("Details", new { id = classeId });
        }
    }
}
