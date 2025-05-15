using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;

        public ChangePasswordCommandHandler(
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

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin)
                && _currentUser.UserId != request.Dto.UserId)
                throw new UnauthorizedAccessException("Not allowed to change this password.");

            var user = await _repo.GetByIdAsync(request.Dto.UserId, ct)
                ?? throw new KeyNotFoundException("User not found.");

            user.ModifierMotDePasse(request.Dto.AncienMotDePasse, request.Dto.NouveauMotDePasse);

            await _uow.SaveChangesAsync(ct);
            foreach (var domainEvent in user.DomainEvents)
                await _mediator.Publish(domainEvent, ct);

            user.ClearDomainEvents();
            return Unit.Value;
        }
    }
}
