using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdatePresence
{
    public class UpdatePresenceCommandHandler
         : IRequestHandler<UpdatePresenceCommand, Result<Guid>>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly IUnitOfWork _uow;
        private readonly IInscriptionManager _manager;
        private readonly IValidator<UpdatePresenceCommand> _validator;

        public UpdatePresenceCommandHandler(
            IInscriptionRepository inscRepo,
            IUnitOfWork uow,
            IInscriptionManager manager,
            IValidator<UpdatePresenceCommand> validator)
        {
            _inscRepo = inscRepo;
            _uow = uow;
            _manager = manager;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(UpdatePresenceCommand cmd, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(cmd, ct);
            if (!validation.IsValid)
                return Result<Guid>.Failure(string.Join(" | ", validation.Errors));

            var insc = await _inscRepo.GetByIdAsync(cmd.InscriptionId, ct);
            if (insc is null)
                return Result<Guid>.Failure("Inscription introuvable.");

            // 1. Construire le nouveau statut
            var statut = StatutPresence.FromString(cmd.Statut);

            // 2. Délégué au manager
            _manager.ModifierPresence(
                inscription: insc,
                presenceId: cmd.PresenceId,
                nouvelleDate: cmd.Date,
                nouvellePeriode: cmd.Periode,
                nouveauStatut: statut
            );

            // 3. Sauvegarde
            await _uow.SaveChangesAsync(ct);
            return Result<Guid>.Success(cmd.PresenceId);
        }
    }
}
