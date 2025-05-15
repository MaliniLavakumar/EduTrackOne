using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;

        public UpdateUserCommandHandler(
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

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
                throw new UnauthorizedAccessException("Only admins can update users.");

            var user = await _repo.GetByIdAsync(request.Dto.Id, ct)
                ?? throw new KeyNotFoundException("User not found.");

            user.ModifierEmail(new Email(request.Dto.Email));
            user.ModifierRole(new RoleUtilisateur((RoleUtilisateur.Role)request.Dto.Role));
            user.ModifierStatut(new StatutUtilisateur((StatutUtilisateur.StatutEnum)request.Dto.Statut));
            // Identifiant change if needed
            //user.ModifierIdentifiant(request.Dto.Identifiant);
            var dto = request.Dto;
            user.MettreÀJourProfil(
                dto.Identifiant,
                new Email(dto.Email),
                new RoleUtilisateur((RoleUtilisateur.Role)dto.Role),
                new StatutUtilisateur((StatutUtilisateur.StatutEnum)dto.Statut)
            );

            await _uow.SaveChangesAsync(ct);
            foreach (var evt in user.DomainEvents)
                await _mediator.Publish(evt, ct);
            user.ClearDomainEvents();
            return Unit.Value;
        }
    }
}
