using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Eleves;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace EduTrackOne.Application.Utilisateurs.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IPasswordHasher<Utilisateur> _passwordHasher;

        public CreateUserCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            IMediator mediator,
             ILogger<CreateUserCommandHandler> logger,
             IPasswordHasher<Utilisateur> passwordHasher
             )
        {
            _repo = repo;
            _uow = uow;
            _currentUser = currentUser;
            _mediator = mediator;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateUserCommand cmd, CancellationToken ct)
        {
            _logger.LogInformation(
                "Début CreateUserCommand pour Identifiant={Identifiant}, Role={Role}, Statut={Statut}",
                cmd.Dto.Identifiant, cmd.Dto.Role, cmd.Dto.Statut);

            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
            {
                _logger.LogWarning(
                    "Utilisateur non autorisé à créer un utilisateur. Utilisateur courant Id={CurrentUserId}, Identifiant courant={CurrentUserIdentifiant}",
                    _currentUser.UserId);
                throw new UnauthorizedAccessException("Only admins can create users.");

            }
          

            var roleVo = new RoleUtilisateur((RoleUtilisateur.Role)cmd.Dto.Role);
            var statutVo = new StatutUtilisateur((StatutUtilisateur.StatutEnum)cmd.Dto.Statut);
            var emailVo = new Email(cmd.Dto.Email);

            // L'entité fire son propre event dans le constructeur
            var user = new Utilisateur(
                Guid.NewGuid(),
                cmd.Dto.Identifiant,
                //cmd.Dto.MotDePasse,
                roleVo,
                statutVo,
                emailVo);
            var motDePasseHash = _passwordHasher.HashPassword(user, cmd.Dto.MotDePasse);
            user.SetMotDePasseHash(motDePasseHash);

            await _repo.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("Utilisateur créé avec succès Id={UserId}", user.Id);


            // Publish domain events
            foreach (var evt in user.DomainEvents)
                await _mediator.Publish(evt, ct);

            user.ClearDomainEvents();

            return user.Id;
        }
    }

}

