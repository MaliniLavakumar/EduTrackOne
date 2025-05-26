using EduTrackOne.API.Models;
using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Application.Utilisateurs.CreateUser;
using EduTrackOne.Application.Utilisateurs.DeleteUser;
using EduTrackOne.Application.Utilisateurs.GetAllUsers;
using EduTrackOne.Application.Utilisateurs.UpdateUser;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize(Roles = "Admin")]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IMediator mediator, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        

        // GET: /User
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            return View(users);
        }

        // GET: /User/Details/{id}
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            // on peut réutiliser GetAll puis FirstOrDefault, ou implémenter GetByIdQuery
            var users = await _mediator.Send(new GetAllUsersQuery());
            var vm = users.FirstOrDefault(u => u.Id == id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET: /User/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Créer un utilisateur";
            return View(new CreateUserViewModel());
        }
        // POST /User/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            // Vérifier que le rôle existe (optionnel)
            if (!await _roleManager.RoleExistsAsync(vm.Role.ToString()))
            {
                ModelState.AddModelError("", $"Le rôle « {vm.Role} » n'existe pas.");
                return View(vm);
            }

            //  Créer l’IdentityUser
            var identityUser = new IdentityUser(vm.Identifiant)
            {
                Email = vm.Email,
                // tu peux ajouter PhoneNumber, etc.
            };
            var createResult = await _userManager.CreateAsync(identityUser, vm.MotDePasse);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }

            // Assigner le rôle
            var roleResult = await _userManager.AddToRoleAsync(identityUser, vm.Role.ToString());
            if (!roleResult.Succeeded)
            {
                // En cas d’erreur, rollback basique
                await _userManager.DeleteAsync(identityUser);
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }
            
            //
            // Persister dans ta table domaine via MediatR
            var dto = new CreateUserDto(
                vm.Identifiant,
                vm.MotDePasse,
                vm.Role.Value,
                vm.Statut.Value,
                vm.Email
            );
            await _mediator.Send(new CreateUserCommand(dto));
            TempData["SuccessMessage"] = "Utilisateur créée avec succès !";

            return RedirectToAction(nameof(Index));
        
        }

        // GET: /Utilisateur/Edit/{id}
        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            var existing = users.FirstOrDefault(u => u.Id == id);
            if (existing == null) return NotFound();
            ViewData["Title"] = "Modifier un utilisateur";
            var vm = new UpdateUserViewModel
            {
                Id = existing.Id,
                Identifiant = existing.Identifiant,
                Email = existing.Email,
                Role = (UserRoleDto)existing.Role,
                Statut = (UserStatusDto)existing.Statut
            };
            return View(vm);
        }
        // POST /User/Edit
        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Pour debug : lister les erreurs en TempData
                var errors = ModelState.Values
                                 .SelectMany(v => v.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToArray();
                TempData["ErrorMessage"] = string.Join(" | ", errors);

                return View(vm);
            }

            if (id != vm.Id) return BadRequest();

            var dto = new UpdateUserDto
            (
                vm.Id,
                vm.Identifiant,
                vm.Role,
                vm.Statut,
                vm.Email
            );

            await _mediator.Send(new UpdateUserCommand(dto));
            return RedirectToAction(nameof(Index));
        }
        // GET: /Utilisateur/ChangePassword/{id}
        [HttpGet("ChangePassword/{id:guid}")]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            var u = users.FirstOrDefault(x => x.Id == id);
            if (u == null) return NotFound();

            var vm = new ChangePasswordViewModel
            {
                UserId = u.Id,
                Identifiant = u.Identifiant
            };
            ViewData["Title"] = $"Changer le mot de passe de « {u.Identifiant} »";
            return View(vm);
        }

        // POST: /User/ChangePassword
        [HttpPost("ChangePassword/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (id != vm.UserId)
                return BadRequest();

            var cmd = new ChangePasswordCommand(new ChangePasswordDto(
                UserId: vm.UserId,
                AncienMotDePasse: vm.AncienMotDePasse,
                NouveauMotDePasse: vm.NouveauMotDePasse
            ));

            await _mediator.Send(cmd);
            TempData["SuccessMessage"] = "Mot de passe mis à jour avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Utilisateur/Delete/{id}
        [HttpGet("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            var u = users.FirstOrDefault(x => x.Id == id);
            if (u == null) return NotFound();

            // Remplit le VM avec l'Id et l'Identifiant
            var vm = new DeleteUserViewModel
            {
                Id = u.Id,
                Identifiant = u.Identifiant
            };
            return View(vm);
        }
        

        // POST: /User/DeleteConfirmed
        [HttpPost("DeleteConfirmed/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return RedirectToAction(nameof(Index));
        }
    }

}
