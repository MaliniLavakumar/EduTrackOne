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

namespace EduTrackOne.Application.Utilisateurs.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;

        public CreateUserCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            IMediator mediator)
        {
            _repo = repo;
            _uow = uow;
            _currentUser = currentUser;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateUserCommand cmd, CancellationToken ct)
        {
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
                throw new UnauthorizedAccessException("Only admins can create users.");

            var roleVo = new RoleUtilisateur((RoleUtilisateur.Role)cmd.Dto.Role);
            var statutVo = new StatutUtilisateur((StatutUtilisateur.StatutEnum)cmd.Dto.Statut);
            var emailVo = new Email(cmd.Dto.Email);

            // L'entité fire son propre event dans le constructeur
            var user = new Utilisateur(
                Guid.NewGuid(),
                cmd.Dto.Identifiant,
                cmd.Dto.MotDePasse,
                roleVo,
                statutVo,
                emailVo);

            await _repo.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            // Publish domain events
            foreach (var evt in user.DomainEvents)
                await _mediator.Publish(evt, ct);

            user.ClearDomainEvents();

            return user.Id;
        }
    }

}

