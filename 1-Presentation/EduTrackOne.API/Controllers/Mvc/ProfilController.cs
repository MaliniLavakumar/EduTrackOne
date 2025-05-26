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

        public ProfilController(IMediator mediator)
            => _mediator = mediator;

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
                return Unauthorized(); 

            if (!ModelState.IsValid) return View("~/Views/User/ChangePassword.cshtml", vm);

            var cmd = new ChangePasswordCommand(new ChangePasswordDto(
                UserId: vm.UserId,
                AncienMotDePasse: vm.AncienMotDePasse,
                NouveauMotDePasse: vm.NouveauMotDePasse
            ));

            await _mediator.Send(cmd);
            TempData["SuccessMessage"] = "Votre mot de passe a été modifié avec succès.";
            return RedirectToAction("ChangePassword"); 
        }
    }
}

