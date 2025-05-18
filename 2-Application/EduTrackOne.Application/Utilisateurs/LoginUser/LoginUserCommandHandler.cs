using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Contracts.DTOs.EduTrackOne.Contracts.DTOs;

namespace EduTrackOne.Application.Utilisateurs.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponseDto>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IMediator mediator,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResponseDto> Handle(LoginUserCommand request, CancellationToken ct)
        {
            // 1. Récupérer l’utilisateur par identifiant
            var identityUser = await _userManager.FindByNameAsync(request.Dto.Identifiant);

            if (identityUser == null)
            {
                // Optionnel : ralentir réponse pour éviter timing attack
                await Task.Delay(500);
                throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");
            }

            // 2. Vérifier le mot de passe
            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Dto.MotDePasse, false);
            if (!signInResult.Succeeded)
                throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");

            // 3. Déclencher l’événement domaine
            var loginEvent = new UserLoggedInEvent(Guid.Parse(identityUser.Id), DateTime.UtcNow);
            await _mediator.Publish(loginEvent, ct);

            // 4. Génération du token JWT
            var roles = await _userManager.GetRolesAsync(identityUser);

            if (identityUser.UserName is null)
                throw new InvalidOperationException("Nom d'utilisateur introuvable.");

            var token = _tokenService.GenerateToken(
                utilisateurId: Guid.Parse(identityUser.Id),
                identifiant: identityUser.UserName,
                roles: roles
            );

            // 5. Retour DTO
            return new LoginUserResponseDto(
                Token: token,
                Identifiant: identityUser.UserName,
                Roles: roles
            );
        }
    }
}
