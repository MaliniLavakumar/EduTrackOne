using EduTrackOne.API.Models;
using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduTrackOne.API.Controllers.Mvc
{


    [Authorize(Roles = "Enseignant")]
    [Route("Profil")]
    public class ProfilController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProfilController> _logger;

        public ProfilController(IMediator mediator,ILogger<ProfilController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
           
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var identifiant = User.Identity?.Name ?? "Mon profil";

            var vm = new ChangePasswordViewModel
            {
                UserId = userId,
                Identifiant = identifiant
            };

            return View("~/Views/User/ChangePassword.cshtml", vm); // réutilisation de la vue
        }

        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (vm.UserId != currentUserId)
            {
                _logger.LogWarning("Tentative non autorisée de changer le mot de passe. UserId soumis: {SubmittedId}, UserId connecté: {CurrentId}", vm.UserId, currentUserId);
                return Unauthorized();
            }


            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Changement de mot de passe refusé pour {UserId} : modèle invalide.", currentUserId);
                return View("~/Views/User/ChangePassword.cshtml", vm);
            }

            try
            {
                var cmd = new ChangePasswordCommand(new ChangePasswordDto(
                    UserId: vm.UserId,
                    AncienMotDePasse: vm.AncienMotDePasse,
                    NouveauMotDePasse: vm.NouveauMotDePasse
                ));

                await _mediator.Send(cmd);

                _logger.LogInformation("Mot de passe modifié avec succès pour l'utilisateur {UserId}", currentUserId);
                TempData["SuccessMessage"] = "Votre mot de passe a été modifié avec succès.";
                return RedirectToAction("ChangePassword");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors du changement de mot de passe pour l'utilisateur {UserId}", currentUserId);
                ModelState.AddModelError(string.Empty, "Une erreur est survenue. Veuillez réessayer.");
                return View("~/Views/User/ChangePassword.cshtml", vm);
            }
        }
    }
}

