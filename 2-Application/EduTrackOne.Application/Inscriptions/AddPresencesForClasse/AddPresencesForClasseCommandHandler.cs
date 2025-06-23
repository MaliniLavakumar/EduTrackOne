using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AddPresencesForClasseCommandHandler> _logger;
        public AddPresencesForClasseCommandHandler(
            IInscriptionRepository inscRepo,
            IPresenceRepository presRepo,
            IInscriptionManager manager,
            IUnitOfWork uow,
            IValidator<AddPresencesForClasseCommand> validator,
            ILogger<AddPresencesForClasseCommandHandler> logger)
        {
            _inscRepo = inscRepo;
            _presRepo = presRepo;
            _manager = manager;
            _uow = uow;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(
           AddPresencesForClasseCommand cmd,
            CancellationToken ct)
        {
            _logger.LogInformation("Début de l'ajout des présences pour la classe {ClasseId} à la date {Date} pour la période {Periode}.", cmd.ClasseId, cmd.Date, cmd.Periode);
            var v = await _validator.ValidateAsync(cmd, ct);
            if (!v.IsValid)
            {
                var errors = string.Join(" | ", v.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation échouée pour AddPresencesForClasseCommand : {Errors}", errors);
                return Result<int>.Failure(errors);
            }
            var inscriptions = await _inscRepo.GetByClasseAsync(cmd.ClasseId, ct);
            if (!inscriptions.Any())
            {
                _logger.LogWarning("Aucune inscription trouvée pour la classe {ClasseId}.", cmd.ClasseId);
                return Result<int>.Failure("Aucune inscription trouvée pour cette classe.");
            }
            Guid? lastPresenceId = null;
            int count = 0;

            foreach (var insc in inscriptions)
            {
                var presenceEleve = cmd.Presences.FirstOrDefault(p => p.EleveId == insc.IdEleve);

                if (presenceEleve is null)
                {
                    _logger.LogDebug("Aucune présence renseignée pour l’élève {EleveId}, inscription ignorée.", insc.IdEleve);
                    continue;
                }

                var presenceId = Guid.NewGuid();
                lastPresenceId = presenceId;
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
                _logger.LogInformation("Présence enregistrée pour élève {EleveId}, statut : {Statut}.", insc.IdEleve, statut.Value);
                count++;
            }
            await _uow.SaveChangesAsync(ct);
            if (count > 0)
            {
                _logger.LogInformation("{Count} présence(s) ajoutée(s) pour la classe {ClasseId}.", count, cmd.ClasseId);
                return Result<int>.Success(count);
            }
            else
            {
                _logger.LogWarning("Aucune présence ajoutée pour la classe {ClasseId}.", cmd.ClasseId);
                return Result<int>.Failure("Aucune présence créée.");
            }
        }
        }
}

