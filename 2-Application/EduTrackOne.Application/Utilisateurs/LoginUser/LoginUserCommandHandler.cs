using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using EduTrackOne.Application.Common.Interfaces;

namespace EduTrackOne.Application.Utilisateurs.LoginUser
{
    public class LoginUserCommandHandler: IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            IMediator mediator,
            IConfiguration config,
            ITokenService tokenService)
        {
            _repo = repo;
            _uow = uow;
            _mediator = mediator;
            _config = config;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken ct)
        {
            // 1. Valider qu’on a bien un utilisateur
            var user = await _repo.GetByIdentifiantAsync(request.Dto.Identifiant, ct)
                       ?? throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");

            // 2. Vérifier le mot de passe
            if (!user.VerifierMotDePasse(request.Dto.MotDePasse))
                throw new UnauthorizedAccessException("Identifiant ou mot de passe incorrect.");

            // 3. Domain Event
            var loginEvent = new UserLoggedInEvent(user.Id, DateTime.UtcNow);
            user.Authentifier();

            await _uow.SaveChangesAsync(ct);

            // 4. Publication de l’événement
            foreach (var evt in user.DomainEvents)
                await _mediator.Publish(evt, ct);
            user.ClearDomainEvents();

            // 5. Création du JWT
            var jwt = _tokenService.GenerateToken(
                utilisateurId: user.Id,
                identifiant: user.Identifiant,
                role: user.Role.Valeur
            );

            return jwt;
        }
    }
}
