using Microsoft.AspNetCore.Mvc;

using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EduTrackOne.Domain.Utilisateurs.Events;

namespace EduTrackOne.API.Controllers.Mvc
{
    public class UtilisateurController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;
        private readonly IMediator _mediator;

        public UtilisateurController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IMediator mediator)
        {
            _signIn = signInManager;
            _users = userManager;
            _mediator = mediator;
        }

        // GET /Utilisateur/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }
        // POST /Utilisateur/Login
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _users.FindByNameAsync(dto.Identifiant);
            if (user == null || !(await _users.CheckPasswordAsync(user, dto.MotDePasse)))
            {
                ModelState.AddModelError("", "Identifiant ou mot de passe incorrect.");
                return View(dto);
            }

            await _signIn.SignInAsync(user, isPersistent: false);

            // (Optionnel) Domain Event
            await _mediator.Publish(new UserLoggedInEvent(Guid.Parse(user.Id), DateTime.UtcNow));

            return RedirectToAction("Index", "Home");
        }


        // GET /Utilisateur/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

