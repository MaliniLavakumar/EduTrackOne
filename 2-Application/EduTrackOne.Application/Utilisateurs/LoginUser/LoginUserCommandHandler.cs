using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Contracts.DTOs.EduTrackOne.Contracts.DTOs;
using Microsoft.Extensions.Logging;

namespace EduTrackOne.Application.Utilisateurs.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponseDto>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginUserCommandHandler> _logger;


        public LoginUserCommandHandler(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IMediator mediator,
            ITokenService tokenService,
             ILogger<LoginUserCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginUserResponseDto> Handle(LoginUserCommand request, CancellationToken ct)
        {
            _logger.LogInformation("LoginUserCommand démarré pour Identifiant={Identifiant}", request.Dto.Identifiant);
            // 1. Récupérer l’utilisateur par identifiant
            var identityUser = await _userManager.FindByNameAsync(request.Dto.Identifiant);

            if (identityUser == null)
            {
                _logger.LogWarning("Échec login : utilisateur introuvable pour Identifiant={Identifiant}", request.Dto.Identifiant);

                // ralentir réponse pour éviter timing attack
                await Task.Delay(500);
                throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");
            }

            // 2. Vérifier le mot de passe
            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Dto.MotDePasse, false);
            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Échec login : mot de passe incorrect pour UserId={UserId}", identityUser.Id);
                throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");
            }
            // 3. Déclencher l’événement domaine
            var userIdGuid = Guid.Parse(identityUser.Id);
            var loginEvent = new UserLoggedInEvent(Guid.Parse(identityUser.Id), DateTime.UtcNow);
            _logger.LogInformation("Publication de UserLoggedInEvent pour UserId={UserId}", userIdGuid);
            await _mediator.Publish(loginEvent, ct);

            // 4. Génération du token JWT
            var roles = await _userManager.GetRolesAsync(identityUser);

            if (identityUser.UserName is null)
            {
                _logger.LogError("Login échoué : UserName null pour UserId={UserId}", identityUser.Id);
                throw new InvalidOperationException("Nom d'utilisateur introuvable.");
            }
            var token = _tokenService.GenerateToken(
                utilisateurId: Guid.Parse(identityUser.Id),
                identifiant: identityUser.UserName,
                roles: roles
            );

            _logger.LogInformation("Login réussi pour UserId={UserId}, NombreRoles={RoleCount}", userIdGuid, roles.Count);
            // 5. Retour DTO
            return new LoginUserResponseDto(
                Token: token,
                Identifiant: identityUser.UserName,
                Roles: roles
            );
        }
    }
}
