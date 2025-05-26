using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Application.Notes.GetNotesByInscription;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("Notes")]
    public class NoteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IInscriptionRepository _inscRepo;
        private readonly IConfiguration _config;
        private readonly IMatiereRepository _matiereRepo;
        public NoteController(IMediator mediator, IInscriptionRepository inscRepo, IConfiguration config, IMatiereRepository matiereRepo )
        {
            _mediator = mediator;
            _inscRepo = inscRepo;
            _config = config;
            _matiereRepo = matiereRepo;
        }
        // GET /Notes/GetNotes/{inscriptionId}?page=1
        [HttpGet("GetNotes/{inscriptionId:guid}")]
        public async Task<IActionResult> GetNotes(
            Guid inscriptionId,
            [FromQuery] int page = 1)

        {
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc is null)
                return NotFound("Inscription introuvable.");
            var query = new GetNotesByInscriptionQuery(
                inscriptionId: inscriptionId,
                pageNumber: page,
                pageSize: 10);

            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();

            ViewBag.EleveName = insc.Eleve.NomComplet.ToString();
            ViewBag.InscriptionId = inscriptionId;
            ViewBag.ClasseID = insc.IdClasse;
            return View(result);
        }
        [HttpGet("Report/{inscriptionId:guid}")]
        public async Task<IActionResult> Report(Guid inscriptionId)
        {
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc is null) return NotFound();

            // Calculer moyennes par matière
            var moyennes = insc.CalculerMoyennesParMatiere();
            var matieres = await _matiereRepo.GetAllAsync();

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
            ViewBag.IsPdf = false;
            return View("Report", vm);
        }

        // 2) Génération PDF
        [HttpGet("ReportPdf/{inscriptionId:guid}")]
        public async Task<IActionResult> ReportPdf(Guid inscriptionId)
        {
            // 1) Charger l’inscription, élève, classe, matières, moyennes
            var insc = await _inscRepo.GetByIdAsync(inscriptionId);
            if (insc is null) return NotFound("Inscription introuvable.");

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

