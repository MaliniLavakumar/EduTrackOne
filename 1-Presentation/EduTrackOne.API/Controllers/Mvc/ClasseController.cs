using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.EnseignantsPrincipaux.GetAllEnseignantsPrincipaux;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Contracts.DTOs;
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
    }
}
