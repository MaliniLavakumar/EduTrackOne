using EduTrackOne.API.Models;
using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.EnseignantsPrincipaux.GetEnseignantPrincipalByEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly IConfiguration _config;

        public HomeController(IMediator mediator, ICurrentUserService currentUser, IConfiguration config)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            string schoolName = _config["School:Name"];
            bool isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                return View(new DashboardViewModel(true,schoolName, null));
            }

            // Récupère l'email de l'utilisateur connecté
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Email introuvable.");
                return View(new DashboardViewModel(false,schoolName, null));
            }

            // 1) Récupérer l'enseignant principal par email
            var enseignantResult = await _mediator.Send(new GetEnseignantPrincipalByEmailQuery(email));
            if (!enseignantResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, enseignantResult.Error);
                return View(new DashboardViewModel(false,schoolName, null));
            }

            var principalId = enseignantResult.Value.Id;

            // 2) Récupérer les classes pour cet enseignant
            var classesFromQuery = await _mediator.Send(new GetClassesByEnseignantPrincipalQuery(principalId));
            var classesVm = classesFromQuery
               .Select(c => new Models.ClassInfoDto(
                   c.Id,
                   c.NomClasse
               ));
            return View(new DashboardViewModel(false,schoolName, classesVm));
        }
    }
}
