using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            IMediator mediator,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _repo = repo;
            _uow = uow;
            _currentUser = currentUser;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            _logger.LogInformation("UpdateUserCommand démarrée pour UserId={UserId}", request.Dto.Id);

            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
            {
                _logger.LogWarning("Accès refusé à la mise à jour d’utilisateur par l’utilisateur courant Id={CurrentUserId}",
                    _currentUser.UserId);
                throw new UnauthorizedAccessException("Only admins can update users.");
            }
            var user = await _repo.GetByIdAsync(request.Dto.Id, ct);
            if (user == null)
            {
                _logger.LogWarning("Update échouée : utilisateur non trouvé pour UserId={UserId}", request.Dto.Id);
                throw new KeyNotFoundException("User not found.");
            }

            _logger.LogInformation("Utilisateur trouvé, mise à jour des champs pour UserId={UserId}", request.Dto.Id);

            user.ModifierEmail(new Email(request.Dto.Email));
            user.ModifierRole(new RoleUtilisateur((RoleUtilisateur.Role)request.Dto.Role));
            user.ModifierStatut(new StatutUtilisateur((StatutUtilisateur.StatutEnum)request.Dto.Statut));

            //user.ModifierIdentifiant(request.Dto.Identifiant);
            var dto = request.Dto;
            user.MettreÀJourProfil(
                dto.Identifiant,
                new Email(dto.Email),
                new RoleUtilisateur((RoleUtilisateur.Role)dto.Role),
                new StatutUtilisateur((StatutUtilisateur.StatutEnum)dto.Statut)
            );

            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("SaveChanges effectué, utilisateur mis à jour en base UserId={UserId}", request.Dto.Id); foreach (var evt in user.DomainEvents)
               
            await _mediator.Publish(evt, ct);

            user.ClearDomainEvents();
            _logger.LogInformation("UpdateUserCommand traité avec succès pour UserId={UserId}", request.Dto.Id);
            return Unit.Value;
        }
    }
}
