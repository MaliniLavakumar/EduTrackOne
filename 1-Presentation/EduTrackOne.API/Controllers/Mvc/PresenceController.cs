using EduTrackOne.Application.Presences;
using EduTrackOne.Domain.Inscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("Presences")]
    public class PresenceController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IInscriptionRepository _inscRepo;

        public PresenceController(IMediator mediator, IInscriptionRepository inscRepo)
        {
            _mediator = mediator;
            _inscRepo = inscRepo;
        }

        // GET /Presence/{inscriptionId}
        [HttpGet("GetPresences/{inscriptionId:guid}")]
        public async Task<IActionResult> GetPresences(Guid inscriptionId)
        {
            // 1) Récupérer l’inscription pour le nom de l’élève et l’id de la classe
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc == null)
                return NotFound("Inscription introuvable.");

            ViewBag.EleveName = insc.Eleve.NomComplet.ToString();
            ViewBag.ClasseId = insc.IdClasse;
            ViewBag.InscriptionId = inscriptionId;

            // 2) Lancer la query
            var result = await _mediator.Send(new GetPresencesByInscriptionQuery(inscriptionId));
            return View(result);
        }
        [HttpGet("Pdf/{inscriptionId:guid}")]
        public async Task<IActionResult> Pdf(Guid inscriptionId)
        {
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc == null) return NotFound("Inscription introuvable.");

            ViewBag.EleveName = insc.Eleve.NomComplet.ToString();
            ViewBag.ClasseId = insc.IdClasse;
            ViewBag.InscriptionId = inscriptionId;

            var dto = await _mediator.Send(new GetPresencesByInscriptionQuery(inscriptionId));

            return new ViewAsPdf("Report", dto)
            {
                FileName = $"Presences_{inscriptionId}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                // on désactive le layout en Razor (voir vue)
            };
        }
    }
}


