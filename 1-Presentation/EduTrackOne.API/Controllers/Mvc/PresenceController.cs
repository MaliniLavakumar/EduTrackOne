using EduTrackOne.Application.Presences;
using EduTrackOne.Domain.Inscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using EduTrackOne.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("Presences")]
    public class PresenceController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IInscriptionRepository _inscRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<PresenceController> _logger;

        public PresenceController(IMediator mediator, IInscriptionRepository inscRepo, IConfiguration config, ILogger<PresenceController> logger)
        {
            _mediator = mediator;
            _inscRepo = inscRepo;
            _config = config;
            _logger = logger;
        }

        // GET /Presence/{inscriptionId}
        [HttpGet("GetPresences/{inscriptionId:guid}")]
        public async Task<IActionResult> GetPresences(Guid inscriptionId)
        {
            // 1) Récupérer l’inscription pour le nom de l’élève et l’id de la classe
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc == null)
            {
                _logger.LogWarning("GetPresences: inscription introuvable pour Id {InscriptionId}", inscriptionId);
                return NotFound("Inscription introuvable.");
            }
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

            //ViewBag.EleveName = insc.Eleve.NomComplet.ToString();
            //ViewBag.ClasseId = insc.IdClasse;
            //ViewBag.InscriptionId = inscriptionId;
            // Récupérer infos
            string eleveName = insc.Eleve.NomComplet.ToString();
            string noImmatricule = insc.Eleve.NoImmatricule ?? string.Empty;
            string classeName = insc.Classe?.Nom.Value ?? string.Empty;

            var presencesDto = await _mediator.Send(new GetPresencesByInscriptionQuery(inscriptionId));

            var vm = new PresenceReportViewModel
            {
                SchoolName = _config["School:Name"] ?? string.Empty,
                ClasseName = classeName,
                EleveName = eleveName,
                NoImmatricule = noImmatricule,
                PeriodesPresentes = presencesDto.PeriodesPresentes,
                Absences = presencesDto.Absences?.ToList() ?? new List<AbsenceDto>()
            };
            _logger.LogInformation("Pdf généré pour {EleveName} (InscriptionId: {InscriptionId})", eleveName, inscriptionId);
            ViewBag.IsPdf = true;
            // Si la vue s’appelle ReportPresencesPdf.cshtml dans Views/Presence/
            return new ViewAsPdf("ReportPresencesPdf", vm)
            {
                FileName = $"Presences_{inscriptionId}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4
            };
        }
    }
}


