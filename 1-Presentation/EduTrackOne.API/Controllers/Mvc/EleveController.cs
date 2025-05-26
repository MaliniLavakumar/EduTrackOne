using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.GetEleveById;
using EduTrackOne.Application.Eleves.GetEleveByImmatricule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EduTrackOne.Application.Eleves.UpdateEleve;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("Eleves")]
    public class EleveController : Controller
    {
        private readonly IMediator _mediator;

        public EleveController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Eleves/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var dto = new CreateEleveDto(
                Prenom: string.Empty,
                Nom: string.Empty,
                DateNaissance: DateTime.Today,
                Sexe: string.Empty,
                Rue: string.Empty,
                CodePostal: string.Empty,
                Ville: string.Empty,
                EmailParent: string.Empty,
                Tel1: string.Empty,
                Tel2: null,
                NoImmatricule: string.Empty
            );
            return View(dto);
        }

        // POST: /Eleves/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEleveDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var cmd = new CreateEleveCommand(
                dto.Prenom,
                dto.Nom,
                dto.DateNaissance,
                dto.Sexe,
                dto.Rue,
                dto.CodePostal,
                dto.Ville,
                dto.EmailParent,
                dto.Tel1,
                dto.Tel2,
                dto.NoImmatricule
            );

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(dto);
            }

            TempData["SuccessMessage"] = "Élève créé avec succès !";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet("ByImmatricule")]
        public async Task<IActionResult> ByImmatricule([FromQuery] string no)
        {
            if (string.IsNullOrWhiteSpace(no))
                return BadRequest(new { message = "Numéro d'immatricule requis." });

            var result = await _mediator.Send(new GetEleveByImmatriculeQuery(no));

            if (!result.IsSuccess)
                return NotFound(new { message = result.Error }); // => "Élève non trouvé."

            var dto = result.Value;

            var nomComplet = $"{dto.Prenom} {dto.Nom}".Trim();

            return Json(new { nomComplet });
        }
        // GET: /Eleves/Details/{id}
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, Guid? classeId)
        {
            var result = await _mediator.Send(new GetEleveByIdQuery(id));
            if (!result.IsSuccess)
                return NotFound(result.Error);

            // EleveDto contient déjà toutes les infos à afficher
            EleveDto dto = result.Value;
            ViewData["ClasseId"] = classeId;
            return View(dto);
        }
        // GET: /Eleves/Edit/{id}?classeId={classeId}
        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, Guid? classeId)
        {
            var result = await _mediator.Send(new GetEleveByIdQuery(id));
            if (!result.IsSuccess)
                return NotFound(result.Error);

           
            var e = result.Value;
            // Préparer le DTO de modification
            var vm = new UpdateEleveDto(
                EleveId: e.Id,
                Rue: e.Adresse.Split(',')[0],      // adapter si votre Adresse.ToString() diffère
                CodePostal: e.Adresse.Split(',')[1].Trim(),
                Ville: e.Adresse.Split(',')[2].Trim(),
                Tel1: e.Tel1,
                Tel2: e.Tel2,
                EmailParent: e.EmailParent
            );

            ViewData["Prenom"] = e.Prenom;
            ViewData["Nom"] = e.Nom;
            ViewData["Naissance"] = e.DateNaissance.ToString("yyyy-MM-dd");
            ViewData["Sexe"] = e.Sexe;
            ViewData["ClasseId"] = classeId;
            return View(vm);
        }

        // POST: /Eleves/Edit/{id}
        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateEleveDto vm, Guid? classeId)
        {
            if (id != vm.EleveId)
                return BadRequest();

            // Réinjecter les readonly dans ViewData via TempData
            ViewData["Prenom"] = TempData["Prenom"];
            ViewData["Nom"] = TempData["Nom"];
            ViewData["Naissance"] = TempData["Naissance"];
            ViewData["Sexe"] = TempData["Sexe"];
            ViewData["ClasseId"] = classeId;

            if (!ModelState.IsValid)
                return View(vm);

            var cmd = new UpdateEleveCommand(
                EleveId: vm.EleveId,
                Rue: vm.Rue,
                CodePostal: vm.CodePostal,
                Ville: vm.Ville,
                Tel1: vm.Tel1,
                Tel2: vm.Tel2,
                EmailParent: vm.EmailParent
            );
            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Coordonnées mises à jour !";

            // Si on vient du détail d'une classe, on y retourne
            if (classeId.HasValue)
                return RedirectToAction("Details", "Classe", new { id = classeId.Value });

            // Sinon, on retourne au dashboard (Home/Index)
            return RedirectToAction("Index", "Home");
        }


    }
}

