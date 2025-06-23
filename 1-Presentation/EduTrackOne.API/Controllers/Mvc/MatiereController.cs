using EduTrackOne.Application.Matieres.CreateMatiere;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Application.Matieres;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    public class MatiereController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MatiereController> _logger;


        public MatiereController(IMediator mediator, ILogger<MatiereController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: /Matiere
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("GET Matiere/Index - début récupération de toutes les matières");

            try
            {
                var result = await _mediator.Send(new GetAllMatieresQuery());
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("GetAllMatieresQuery échoué: {Error}", result.Error);
                    TempData["ErrorMessage"] = result.Error;                    
                    return View(Enumerable.Empty<MatiereDto>());
                }
                var matieres = result.Value;
                // Si besoin, compter ou loguer le nombre d’éléments
                int count = matieres is ICollection<MatiereDto> coll ? coll.Count : matieres.Count();
                _logger.LogInformation("GetAllMatieresQuery réussi: {Count} matières récupérées", count);

                return View(matieres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans GET Matiere/Index");
                TempData["ErrorMessage"] = "Erreur interne lors de la récupération des matières.";
                return View(Enumerable.Empty<MatiereDto>());
            }

        }

        // GET: /Matiere/Create
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("GET Matiere/Create - début affichage du formulaire de création");
            var dto = new CreateMatiereDto(NomMatiere: string.Empty);
            _logger.LogInformation("GET Matiere/Create - fin préparation CreateMatiereDto par défaut");
            return View(dto);
        }

        // POST: /Matiere/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMatiereDto dto)
        {
            _logger.LogInformation("POST Matiere/Create - début: NomMatiere={NomMatiere}", dto.NomMatiere);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST Matiere/Create - ModelState invalide pour NomMatiere={NomMatiere}", dto.NomMatiere);
                return View(dto);
            }

            var cmd = new CreateMatiereCommand(dto.NomMatiere);
            try
            {
                var result = await _mediator.Send(cmd);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("CreateMatiereCommand échoué pour NomMatiere={NomMatiere}: {Error}", dto.NomMatiere, result.Error);
                    ModelState.AddModelError(string.Empty, result.Error);
                    return View(dto);
                }

                _logger.LogInformation("Matière créée avec succès: NomMatiere={NomMatiere}", dto.NomMatiere);
                TempData["SuccessMessage"] = "Matière créée avec succès.";
                _logger.LogInformation("Redirection vers Index après création de la matière");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans POST Matiere/Create pour NomMatiere={NomMatiere}", dto.NomMatiere);
                ModelState.AddModelError(string.Empty, "Erreur interne lors de la création de la matière.");
                return View(dto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation("GET Matiere/Details - début pour MatiereId={MatiereId}", id);

            try
            {
                _logger.LogInformation("GET Matiere/Details - redirection vers Index (pas de GetMatiereByIdQuery implémenté)");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans GET Matiere/Details pour MatiereId={MatiereId}", id);
                TempData["ErrorMessage"] = "Erreur interne.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

