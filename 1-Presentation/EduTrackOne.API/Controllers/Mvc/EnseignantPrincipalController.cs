using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("EnseignantsPrincipaux")]
    public class EnseignantPrincipalController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EnseignantPrincipalController> _logger;
        public EnseignantPrincipalController(IMediator mediator, ILogger<EnseignantPrincipalController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        // GET: /EnseignantsPrincipaux/Create
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("GET Create EnseignantPrincipal - début");
            _logger.LogInformation("GET Create EnseignantPrincipal - affichage de la vue Create");
            return View();
        }

        // POST: /EnseignantsPrincipaux/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEnseignantPrincipalDto dto)
        {
            _logger.LogInformation("POST Create EnseignantPrincipal - début: Prenom={Prenom}, Nom={Nom}, Email={Email}",
                dto.Prenom, dto.Nom, dto.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST Create EnseignantPrincipal - ModelState invalide");
                return View(dto);
            }

            var cmd = new CreateEnseignantPrincipalCommand(
                dto.Prenom,
                dto.Nom,
                dto.Email);

            try
            {
                var result = await _mediator.Send(cmd);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("CreateEnseignantPrincipalCommand échoué: {Error}", result.Error);
                    ModelState.AddModelError(string.Empty, result.Error);
                    return View(dto);
                }

                // Si CreateEnseignantPrincipalCommand retourne un Id ou info, on peut le logger :
                _logger.LogInformation("EnseignantPrincipal créé avec succès: Id={IdResult}", result.Value);
                TempData["SuccessMessage"] = "Enseignant principal créé avec succès !";
                _logger.LogInformation("Redirection vers Home/Index après création EnseignantPrincipal");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue lors de CreateEnseignantPrincipalCommand pour Email={Email}", dto.Email);
                ModelState.AddModelError(string.Empty, "Erreur interne lors de la création de l’enseignant principal.");
                return View(dto);
            }
        }

    }

}
