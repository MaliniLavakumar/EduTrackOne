using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;

        public DeleteUserCommandHandler(
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

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
        {
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
                throw new UnauthorizedAccessException("Only admins can delete users.");

            var user = await _repo.GetByIdAsync(request.UserId, ct)
                ?? throw new KeyNotFoundException("User not found.");
            
            user.Supprimer();
            _repo.Delete(user);

            await _uow.SaveChangesAsync(ct);
            foreach (var domainEvent in user.DomainEvents)
                await _mediator.Publish(domainEvent, ct);

            user.ClearDomainEvents();
            return Unit.Value;
        }
    }

}
