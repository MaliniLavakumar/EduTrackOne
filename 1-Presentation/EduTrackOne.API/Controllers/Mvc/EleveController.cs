using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.GetEleveByImmatricule;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    }
}

