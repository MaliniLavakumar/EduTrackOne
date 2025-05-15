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

namespace EduTrackOne.Application.Inscriptions.AddPresencesForClasse
{
    public class AddPresencesForClasseCommandHandler : IRequestHandler<AddPresencesForClasseCommand, Result<int>>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly IPresenceRepository _presRepo;
        private readonly IInscriptionManager _manager;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<AddPresencesForClasseCommand> _validator;

        public AddPresencesForClasseCommandHandler(
            IInscriptionRepository inscRepo,
            IPresenceRepository presRepo,
            IInscriptionManager manager,
            IUnitOfWork uow,
            IValidator<AddPresencesForClasseCommand> validator)
        {
            _inscRepo = inscRepo;
            _presRepo = presRepo;
            _manager = manager;
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(
           AddPresencesForClasseCommand cmd,
            CancellationToken ct)
        {
            var v = await _validator.ValidateAsync(cmd, ct);
            if (!v.IsValid)
                return Result<int>.Failure(string.Join(" | ", v.Errors));

            var inscriptions = await _inscRepo.GetByClasseAsync(cmd.ClasseId, ct);
            if (!inscriptions.Any())
                return Result<int>.Failure("Aucune inscription trouvée pour cette classe.");

            Guid? lastPresenceId = null;
            int count = 0;

            foreach (var insc in inscriptions)
            {
                var presenceEleve = cmd.Presences.FirstOrDefault(p => p.EleveId == insc.IdEleve);

                if (presenceEleve is null)
                    continue; // élève non renseigné

                var presenceId = Guid.NewGuid();
                var statut = presenceEleve.Statut == "Present"
                    ? new StatutPresence(StatutPresence.StatutEnum.Present)
                    : new StatutPresence(StatutPresence.StatutEnum.Absent);

                var presence = new Presence(
                presenceId,
                cmd.Date,
                cmd.Periode,
                statut,
                insc.Id
            );

                _manager.MarquerPresence(insc, presence);
                await _presRepo.AddAsync(presence, ct);               
                count++;
            }
            await _uow.SaveChangesAsync(ct);
            return lastPresenceId.HasValue
            ? Result<int>.Success(count)
            : Result<int>.Failure("Aucune présence créée.");
        }
    }
}

