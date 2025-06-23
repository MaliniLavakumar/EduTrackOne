using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.EnseignantsPrincipaux.GetEnseignantPrincipalByEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using EduTrackOne.Contracts.DTOs;
using ViewClassInfoDto = EduTrackOne.API.Models.ClassInfoDto;
using AppClassInfoDto = EduTrackOne.Contracts.DTOs.ClassInfoDto;


namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IMediator mediator, ICurrentUserService currentUser, IConfiguration config, ILogger<HomeController> logger)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _config = config;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("GET Home/Index - début");
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("Utilisateur non authentifié, redirection vers Utilisateur/Login");
                return RedirectToAction("Login", "Utilisateur");
            }
            string schoolName = _config["School:Name"];
            bool isAdmin = User.IsInRole("Admin");
            _logger.LogInformation("Utilisateur authentifié. Email={EmailClaim}, IsAdmin={IsAdmin}",
              User.FindFirstValue(ClaimTypes.Email), isAdmin);

            if (isAdmin)
            {
                _logger.LogInformation("Utilisateur en rôle Admin, affichage du dashboard admin");
                return View(new DashboardViewModel(true, schoolName, null));
            }

            // Récupère l'email de l'utilisateur connecté
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Index: Email introuvable dans les claims de l'utilisateur");
                ModelState.AddModelError(string.Empty, "Email introuvable.");
                return View(new DashboardViewModel(false, schoolName, null));
            }
            _logger.LogInformation("Index: Email récupéré = {Email}", email);

            try
            {
                // 1) Récupérer l'enseignant principal par email
                _logger.LogInformation("Envoi de GetEnseignantPrincipalByEmailQuery pour email={Email}", email);
                var enseignantResult = await _mediator.Send(new GetEnseignantPrincipalByEmailQuery(email));
                if (!enseignantResult.IsSuccess)
                {
                    _logger.LogWarning("GetEnseignantPrincipalByEmailQuery échoué pour email={Email}: {Error}", email, enseignantResult.Error);
                    ModelState.AddModelError(string.Empty, enseignantResult.Error);
                    return View(new DashboardViewModel(false, schoolName, null));
                }

                var principalId = enseignantResult.Value.Id;
                _logger.LogInformation("Enseignant principal trouvé: Id={PrincipalId}", principalId);

                // 2) Récupérer les classes pour cet enseignant
                _logger.LogInformation("Envoi de GetClassesByEnseignantPrincipalQuery pour PrincipalId={PrincipalId}", principalId);
                var classesFromQuery = await _mediator.Send(new GetClassesByEnseignantPrincipalQuery(principalId));
                if (classesFromQuery == null)
                {
                    _logger.LogWarning("GetClassesByEnseignantPrincipalQuery a renvoyé null pour PrincipalId={PrincipalId}", principalId);
                    // On peut continuer avec liste vide
                    classesFromQuery = Enumerable.Empty<AppClassInfoDto>(); 
                }
                var classesVm = classesFromQuery
                   .Select(c => new Models.ClassInfoDto(
                       c.Id,
                       c.NomClasse
                   ))
                   .ToList();
                _logger.LogInformation("GetClassesByEnseignantPrincipalQuery réussi: {Count} classes récupérées pour PrincipalId={PrincipalId}",
                    classesVm.Count, principalId);

                return View(new DashboardViewModel(false, schoolName, classesVm));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans Home/Index pour email={Email}", email);
                ModelState.AddModelError(string.Empty, "Une erreur interne est survenue.");
                return View(new DashboardViewModel(false, schoolName, null));
            }
        }
        public IActionResult Privacy()
        {
            _logger.LogInformation("GET Home/Privacy - affichage de la vue Privacy");
            return View();
        }

    }
}
