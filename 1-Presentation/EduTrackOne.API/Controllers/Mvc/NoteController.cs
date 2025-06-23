using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Application.Notes.GetNotesByInscription;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("Notes")]
    public class NoteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IInscriptionRepository _inscRepo;
        private readonly IConfiguration _config;
        private readonly IMatiereRepository _matiereRepo;
        private readonly ILogger<NoteController> _logger;
        public NoteController(IMediator mediator, IInscriptionRepository inscRepo, IConfiguration config, IMatiereRepository matiereRepo, ILogger<NoteController> logger)
        {
            _mediator = mediator;
            _inscRepo = inscRepo;
            _config = config;
            _matiereRepo = matiereRepo;
            _logger = logger;
        }
        // GET /Notes/GetNotes/{inscriptionId}?page=1
        [HttpGet("GetNotes/{inscriptionId:guid}")]
        public async Task<IActionResult> GetNotes(
            Guid inscriptionId,
            [FromQuery] int page = 1)

        {
            _logger.LogInformation("GET Note/GetNotes - début pour InscriptionId={InscriptionId}, page={Page}", inscriptionId, page);

            try
            {
                var insc = await _inscRepo.GetByIdAsync(inscriptionId);
                if (insc is null)
                {
                    _logger.LogWarning("GetNotes: Inscription introuvable pour InscriptionId={InscriptionId}", inscriptionId);
                    return NotFound("Inscription introuvable.");
                }
                _logger.LogInformation("GetNotes: Inscription trouvée pour InscriptionId={InscriptionId}, Eleve={Eleve}, ClasseId={ClasseId}",
                    inscriptionId, insc.Eleve?.NomComplet.ToString(), insc.IdClasse);

                var query = new GetNotesByInscriptionQuery(
                    inscriptionId: inscriptionId,
                    pageNumber: page,
                    pageSize: 10);

                _logger.LogInformation("Envoi de GetNotesByInscriptionQuery pour InscriptionId={InscriptionId}, page={Page}", inscriptionId, page);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    _logger.LogWarning("GetNotesByInscriptionQuery a renvoyé null pour InscriptionId={InscriptionId}, page={Page}", inscriptionId, page);
                    return NotFound();
                }

                ViewBag.EleveName = insc.Eleve.NomComplet.ToString();
                ViewBag.InscriptionId = inscriptionId;
                ViewBag.ClasseID = insc.IdClasse;
                _logger.LogInformation("GET Note/GetNotes - fin pour InscriptionId={InscriptionId}", inscriptionId);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans GET Note/GetNotes pour InscriptionId={InscriptionId}, page={Page}", inscriptionId, page);
                return StatusCode(500, "Erreur interne lors de la récupération des notes.");
            }
        }
        [HttpGet("Report/{inscriptionId:guid}")]
        public async Task<IActionResult> Report(Guid inscriptionId)
        {
            _logger.LogInformation("GET Note/Report - début pour InscriptionId={InscriptionId}", inscriptionId);
          

            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc is null)
            {
                _logger.LogWarning("Report: Inscription introuvable pour InscriptionId={InscriptionId}", inscriptionId);
                return NotFound("Inscription introuvable.");
            }

            _logger.LogInformation("Report: Inscription chargée pour InscriptionId={InscriptionId}, Eleve={Eleve}",
                                inscriptionId, insc.Eleve?.NomComplet.ToString());
            // Calculer moyennes par matière
            var moyennes = insc.CalculerMoyennesParMatiere();
            _logger.LogInformation("Report: Moyennes calculées pour InscriptionId={InscriptionId}, NombreMatieres={Count}", inscriptionId, moyennes.Count);

            var matieres = await _matiereRepo.GetAllAsync();
            _logger.LogInformation("Report: Matières chargées, total matières={Count}", matieres.Count);

            var vm = new BulletinViewModel
            {
                SchoolName = _config["School:Name"],
                ClasseName = insc.Classe.Nom.Value,
                EleveName = insc.Eleve.NomComplet.ToString(),
                NoImmatricule = insc.Eleve.NoImmatricule,
                Matieres = moyennes.Select(kv => new MatiereBulletinDto
                {
                    MatiereNom = matieres.First(m => m.Id == kv.Key).Nom.Value,
                    Moyenne = kv.Value
                }).ToList()
            };
            _logger.LogInformation("Report: ViewModel construit pour InscriptionId={InscriptionId}", inscriptionId);
        
        ViewBag.IsPdf = false;
            _logger.LogInformation("GET Note/Report - fin pour InscriptionId={InscriptionId}", inscriptionId);
            return View("Report", vm);
        }

        // 2) Génération PDF
        [HttpGet("ReportPdf/{inscriptionId:guid}")]
        public async Task<IActionResult> ReportPdf(Guid inscriptionId)
        {
            _logger.LogInformation("GET Note/ReportPdf - début pour InscriptionId={InscriptionId}", inscriptionId);

            // 1) Charger l’inscription, élève, classe, matières, moyennes
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc is null)
            {
                _logger.LogWarning("ReportPdf: Inscription introuvable pour InscriptionId={InscriptionId}", inscriptionId);
                return NotFound("Inscription introuvable.");
            }
            _logger.LogInformation("ReportPdf: Inscription chargée pour InscriptionId={InscriptionId}", inscriptionId);


            var moyennes = insc.CalculerMoyennesParMatiere();
            var matieres = await _matiereRepo.GetAllAsync();
            var classeDto = (await _mediator.Send(new GetClasseByIdQuery(insc.IdClasse))).Value;

            // 2) Construire le ViewModel
            var vm = new BulletinViewModel
            {
                SchoolName = _config["School:Name"],
                ClasseName = classeDto.NomClasse,
                EleveName = insc.Eleve.NomComplet.ToString(),
                NoImmatricule = insc.Eleve.NoImmatricule,
                Matieres = moyennes.Select(kv => new MatiereBulletinDto
                {
                    MatiereNom = matieres.First(m => m.Id == kv.Key).Nom.Value,
                    Moyenne = kv.Value
                }).ToList()
            };

            _logger.LogInformation("Génération PDF pour InscriptionId={InscriptionId}", inscriptionId);
            // 3) Lance la génération PDF
            return new ViewAsPdf("_PdfReport", vm)
            {
                FileName = $"Bulletin_{inscriptionId}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4
            };
        }

    }
}

