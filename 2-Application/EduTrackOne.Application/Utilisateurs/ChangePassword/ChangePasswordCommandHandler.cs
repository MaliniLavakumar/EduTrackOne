using EduTrackOne.Application.Common;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly IPasswordHasher<Utilisateur> _passwordHasher;


        public ChangePasswordCommandHandler(
            IUtilisateurRepository repo,
            IUnitOfWork uow,
            ICurrentUserService currentUser,
            IMediator mediator,
            ILogger<ChangePasswordCommandHandler> logger,
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

        public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            _logger.LogInformation("Tentative de changement de mot de passe pour l'utilisateur {UserId}", request.Dto.UserId);
            if (!_currentUser.IsInRole(RoleUtilisateur.Role.Admin)
                && _currentUser.UserId != request.Dto.UserId)
            {
                _logger.LogWarning("Accès refusé : l'utilisateur {CurrentUserId} a tenté de changer le mot de passe de l'utilisateur {TargetUserId}",
                  _currentUser.UserId, request.Dto.UserId);
                return Result<Unit>.Failure("Vous n’êtes pas autorisé à changer ce mot de passe.");
            }
            var user = await _repo.GetByIdAsync(request.Dto.UserId, ct);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur introuvable avec l'identifiant {UserId}", request.Dto.UserId);
                return Result<Unit>.Failure("Utilisateur introuvable.");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.MotDePasseHash, request.Dto.AncienMotDePasse);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Ancien mot de passe incorrect pour l'utilisateur {UserId}", request.Dto.UserId);
                return Result<Unit>.Failure("Ancien mot de passe incorrect.");
            }

            if (request.Dto.AncienMotDePasse == request.Dto.NouveauMotDePasse)
            {
                _logger.LogWarning("Mot de passe inchangé pour l'utilisateur {UserId}", request.Dto.UserId);
                return Result<Unit>.Failure("Le nouveau mot de passe doit être différent de l’ancien.");
            }

            try
            {
                var nouveauHash = _passwordHasher.HashPassword(user, request.Dto.NouveauMotDePasse);
                user.ModifierMotDePasse(nouveauHash);

                _logger.LogInformation("Mot de passe changé avec succès pour l'utilisateur {UserId}", request.Dto.UserId);
                await _uow.SaveChangesAsync(ct);

                foreach (var domainEvent in user.DomainEvents)
                    await _mediator.Publish(domainEvent, ct);

                user.ClearDomainEvents();

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors du changement de mot de passe pour l'utilisateur {UserId}", request.Dto.UserId);
                return Result<Unit>.Failure("Une erreur est survenue lors du changement de mot de passe.");
            }
        }
    }
}
