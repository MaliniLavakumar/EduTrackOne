using Microsoft.AspNetCore.Mvc;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Domain.Utilisateurs;
using Microsoft.AspNetCore.Authorization;

namespace EduTrackOne.API.Controllers.Mvc
{
    
    public class UtilisateurController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;
        private readonly IMediator _mediator;
        private readonly IUtilisateurRepository _utilisateurRepo;
        private readonly ILogger<UtilisateurController> _logger;
        public UtilisateurController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IMediator mediator, IUtilisateurRepository utilisateurRepo,
            ILogger<UtilisateurController> logger)
        {
            _signIn = signInManager;
            _users = userManager;
            _mediator = mediator;
            _utilisateurRepo = utilisateurRepo;
            _logger = logger;
        }

        // GET /Utilisateur/Login
       
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("Utilisateur déjà authentifié. Déconnexion forcée.");
                // Supprime le cookie de session existant (même restauré)
                await _signIn.SignOutAsync();
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        // POST /Utilisateur/Login
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> Login(LoginDto dto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Échec de connexion : modèle invalide.");
                return View(dto);
            }

            var user = await _users.FindByNameAsync(dto.Identifiant);
            if (user == null)
            {
                _logger.LogWarning("Tentative de connexion avec un identifiant inconnu : {Identifiant}", dto.Identifiant);
                ModelState.AddModelError("", "Identifiant ou mot de passe incorrect.");
                return View(dto);
            }

            // 2. Vérifier l’entité domaine pour le statut
             var domainUser = await _utilisateurRepo.GetByIdentifiantAsync(user.UserName);
            if (domainUser == null)
            {
                _logger.LogWarning("Utilisateur ASP.NET trouvé, mais pas dans le domaine : {Identifiant}", user.UserName);
                // Considérer comme échec de connexion
                ModelState.AddModelError("", "Identifiant ou mot de passe incorrect.");
                return View(dto);
            }

            // 3. Vérifier le statut : si inactif, refuser la connexion
            if (domainUser.Statut.Value == StatutUtilisateur.StatutEnum.Inactif)
            {
                _logger.LogWarning("Connexion refusée pour un utilisateur inactif : {Identifiant}", user.UserName);
                ModelState.AddModelError("", "Ce compte est désactivé. Veuillez contacter l’administrateur.");
                return View(dto);
            }

            // 4. Vérifier le mot de passe
            var passwordValid = await _users.CheckPasswordAsync(user, dto.MotDePasse);
            if (!passwordValid)
            {
                _logger.LogWarning("Mot de passe incorrect pour l’utilisateur : {Identifiant}", user.UserName);
                ModelState.AddModelError("", "Identifiant ou mot de passe incorrect.");
                return View(dto);
            }

            await _signIn.SignInAsync(user, isPersistent: false);

            // (Optionnel) Domain Event
            await _mediator.Publish(new UserLoggedInEvent(Guid.Parse(user.Id), DateTime.UtcNow));

            TempData["WelcomeMessage"] = $"Bienvenue, {User.Identity.Name}";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }


        // GET /Utilisateur/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Utilisateur déconnecté.");
            await _signIn.SignOutAsync();
            return RedirectToAction("Login");
        }


    }
}

