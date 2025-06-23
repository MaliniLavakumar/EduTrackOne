using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.GetEleveById;
using EduTrackOne.Application.Eleves.GetEleveByImmatricule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EduTrackOne.Application.Eleves.UpdateEleve;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("Eleves")]
    public class EleveController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EleveController> _logger;

        public EleveController(IMediator mediator, ILogger<EleveController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: /Eleves/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            _logger.LogInformation("GET Create Eleve - début");
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
            _logger.LogInformation("GET Create Eleve - fin (préparation DTO par défaut)");
            return View(dto);
        }

        // POST: /Eleves/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEleveDto dto)
        {
            _logger.LogInformation("POST Create Eleve - début: Immatricule={Immatricule}, Nom={Nom}, Prenom={Prenom}",
                dto.NoImmatricule, dto.Nom, dto.Prenom);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST Create Eleve - ModelState invalide");
                return View(dto);
            }

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

            try
            {
                var result = await _mediator.Send(cmd);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("CreateEleveCommand échoué: {Error}", result.Error);
                    ModelState.AddModelError(string.Empty, result.Error);
                    return View(dto);
                }
                _logger.LogInformation("Eleve créé avec succès: Id={EleveId}", result.Value);
                TempData["SuccessMessage"] = "Élève créé avec succès !";
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue lors de CreateEleveCommand pour Immatricule={Immatricule}", dto.NoImmatricule);
                ModelState.AddModelError(string.Empty, "Erreur interne lors de la création.");
                return View(dto);
            }
        }
        [HttpGet("ByImmatricule")]
        public async Task<IActionResult> ByImmatricule([FromQuery] string no)
        {
            _logger.LogInformation("GET ByImmatricule - début pour Immatricule={Immatricule}", no);
            if (string.IsNullOrWhiteSpace(no))
            {
                _logger.LogWarning("ByImmatricule appelé sans param Immatricule");
                return BadRequest(new { message = "Numéro d'immatricule requis." });
            }


            try
            {
                var result = await _mediator.Send(new GetEleveByImmatriculeQuery(no));
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("GetEleveByImmatriculeQuery: élève non trouvé pour Immatricule={Immatricule}", no);
                    return NotFound(new { message = result.Error });
                }

                var dto = result.Value;
                var nomComplet = $"{dto.Prenom} {dto.Nom}".Trim();
                _logger.LogInformation("ByImmatricule réussi pour Immatricule={Immatricule}, EleveId={EleveId}", no, dto.Id);
                return Json(new { nomComplet });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans ByImmatricule pour Immatricule={Immatricule}", no);
                return StatusCode(500, new { message = "Erreur interne" });
            }
        }
        // GET: /Eleves/Details/{id}
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, Guid? classeId)
        {
            _logger.LogInformation("GET Details Eleve - début pour EleveId={EleveId}, ClasseIdContext={ClasseId}", id, classeId);
            try
            {
                var result = await _mediator.Send(new GetEleveByIdQuery(id));
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("GetEleveByIdQuery: élève non trouvé pour EleveId={EleveId}: {Error}", id, result.Error);
                    return NotFound(result.Error);
                }

                EleveDto dto = result.Value;
                ViewData["ClasseId"] = classeId;
                _logger.LogInformation("GET Details Eleve - fin pour EleveId={EleveId}", id);
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans Details pour EleveId={EleveId}", id);
                return StatusCode(500, "Erreur interne");
            }
        }
        // GET: /Eleves/Edit/{id}?classeId={classeId}
        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, Guid? classeId)
        {
            _logger.LogInformation("GET Edit Eleve - début pour EleveId={EleveId}, ClasseIdContext={ClasseId}", id, classeId);
            try
            {
                var result = await _mediator.Send(new GetEleveByIdQuery(id));
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("GetEleveByIdQuery échoué pour Edit EleveId={EleveId}: {Error}", id, result.Error);
                    return NotFound(result.Error);
                }


                var e = result.Value;
                // Préparer le DTO de modification
                var vm = new UpdateEleveDto(
                    EleveId: e.Id,
                    Rue: e.Adresse.Split(',')[0],
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
                ViewData["Immatricule"] = e.NoImmatricule;

                _logger.LogInformation("GET Edit Eleve - fin préparation ViewModel pour EleveId={EleveId}", id);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans GET Edit Eleve pour EleveId={EleveId}", id);
                return StatusCode(500, "Erreur interne");
            }
        }

            // POST: /Eleves/Edit/{id}
            [HttpPost("Edit/{id:guid}")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, UpdateEleveDto vm, Guid? classeId)
            {
            _logger.LogInformation("POST Edit Eleve - début pour EleveId={EleveId}, ClasseIdContext={ClasseId}", id, classeId);
            if (id != vm.EleveId)
            {
                _logger.LogWarning("POST Edit Eleve - mauvais EleveId (route {RouteId} != vm.EleveId {VmId})", id, vm.EleveId);
                return BadRequest();
            }

            // Réinjecter les readonly dans ViewData via TempData
            ViewData["Prenom"] = TempData["Prenom"];
                ViewData["Nom"] = TempData["Nom"];
                ViewData["Naissance"] = TempData["Naissance"];
                ViewData["Sexe"] = TempData["Sexe"];
                ViewData["ClasseId"] = classeId;
                ViewData["Immatricule"] = TempData["Immatricule"];

                if (!ModelState.IsValid)
                {
                    await RemplirReadonlyDansViewData(id, classeId);
                    return View(vm);
                }

                var cmd = new UpdateEleveCommand(
                    EleveId: vm.EleveId,
                    Rue: vm.Rue,
                    CodePostal: vm.CodePostal,
                    Ville: vm.Ville,
                    Tel1: vm.Tel1,
                    Tel2: vm.Tel2,
                    EmailParent: vm.EmailParent
                );
            try
            {
                var result = await _mediator.Send(cmd);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("UpdateEleveCommand échoué pour EleveId={EleveId}: {Error}", vm.EleveId, result.Error);
                    ModelState.AddModelError(string.Empty, result.Error);
                    return View(vm);
                }

                _logger.LogInformation("Coordonnées mises à jour pour EleveId={EleveId}", vm.EleveId);
                TempData["SuccessMessage"] = "Coordonnées mises à jour !";

                // Si on vient du détail d'une classe, on y retourne
                if (classeId.HasValue)
                {
                    _logger.LogInformation("Redirection vers Details Classe après Edit EleveId={EleveId}, ClasseId={ClasseId}", vm.EleveId, classeId.Value);
                    return RedirectToAction("Details", "Classe", new { id = classeId.Value });
                }

                _logger.LogInformation("Redirection vers Home/Index après Edit EleveId={EleveId}", vm.EleveId);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue lors de UpdateEleveCommand pour EleveId={EleveId}", vm.EleveId);
                ModelState.AddModelError(string.Empty, "Erreur interne lors de la mise à jour.");
                return View(vm);
            }
        }
        private async Task RemplirReadonlyDansViewData(Guid eleveId, Guid? classeId)
        {
            _logger.LogInformation("RemplirReadonlyDansViewData - début pour EleveId={EleveId}", eleveId);
            try
            {
                var result = await _mediator.Send(new GetEleveByIdQuery(eleveId));
                if (result.IsSuccess)
                {
                    var e = result.Value;
                    ViewData["Prenom"] = e.Prenom;
                    ViewData["Nom"] = e.Nom;
                    ViewData["Naissance"] = e.DateNaissance.ToString("yyyy-MM-dd");
                    ViewData["Sexe"] = e.Sexe;
                    ViewData["ClasseId"] = classeId;
                    ViewData["Immatricule"] = e.NoImmatricule;
                    // Éventuellement fixer ViewData["Title"] si on l’utilise ici
                    ViewData["Title"] = $"Modifier l'élève : {e.Prenom} {e.Nom}";
                }
                else
                {
                    // En cas d’erreur (éventuellement élève introuvable),
                    // on vide ou met des valeurs par défaut
                    ViewData["Prenom"] = "";
                    ViewData["Nom"] = "";
                    ViewData["Naissance"] = "";
                    ViewData["Sexe"] = "";
                    ViewData["ClasseId"] = classeId;
                    ViewData["Immatricule"] = "";
                    ViewData["Title"] = "Modifier l'élève";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans RemplirReadonlyDansViewData pour EleveId={EleveId}", eleveId);
                // On peut laisser ViewData partiellement vide
                ViewData["Prenom"] = "";
                ViewData["Nom"] = "";
                ViewData["Naissance"] = "";
                ViewData["Sexe"] = "";
                ViewData["ClasseId"] = classeId;
                ViewData["Immatricule"] = "";
                ViewData["Title"] = "Modifier l'élève";
            }

        }


    }
}

