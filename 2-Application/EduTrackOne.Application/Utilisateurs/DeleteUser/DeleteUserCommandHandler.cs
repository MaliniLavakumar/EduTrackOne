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
using Microsoft.Extensions.Logging;

namespace EduTrackOne.Application.Utilisateurs.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteUserCommandHandler> _logger;


        public DeleteUserCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            IMediator mediator,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _repo = repo;
            _uow = uow;
            _currentUser = currentUser;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
        {
            _logger.LogInformation("DeleteUserCommand commencé pour UserId={UserId}", request.UserId);
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin))
            {
                _logger.LogWarning(
                    "Accès refusé à la suppression d’utilisateur par l’utilisateur courant Id={CurrentUserId}",
                    _currentUser.UserId);
                throw new UnauthorizedAccessException("Only admins can delete users.");
            }

            var user = await _repo.GetByIdAsync(request.UserId, ct);
             if (user == null)
            {
                _logger.LogWarning("Suppression échouée : utilisateur non trouvé pour UserId={UserId}", request.UserId);
                throw new KeyNotFoundException("User not found.");
            }

            _logger.LogInformation("Utilisateur trouvé, préparation suppression UserId={UserId}", request.UserId);


            user.Supprimer();
            _repo.Delete(user);

            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("SaveChanges effectué, utilisateur supprimé en base UserId={UserId}", request.UserId);
            foreach (var domainEvent in user.DomainEvents)
                await _mediator.Publish(domainEvent, ct);

            user.ClearDomainEvents();
            return Unit.Value;
        }
    }

}
