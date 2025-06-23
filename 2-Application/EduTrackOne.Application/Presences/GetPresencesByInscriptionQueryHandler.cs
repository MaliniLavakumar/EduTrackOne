using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Presences
{
    public class GetPresencesByInscriptionQueryHandler
         : IRequestHandler<GetPresencesByInscriptionQuery, GetPresencesByInscriptionDto>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly ILogger<GetPresencesByInscriptionQueryHandler> _logger;

        public GetPresencesByInscriptionQueryHandler(IInscriptionRepository inscRepo, ILogger<GetPresencesByInscriptionQueryHandler> logger)
        {
            _inscRepo = inscRepo;
            _logger = logger;
        }

        public async Task<GetPresencesByInscriptionDto> Handle(
            GetPresencesByInscriptionQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Traitement de GetPresencesByInscriptionQuery pour InscriptionId: {InscriptionId}", request.InscriptionId);
            var inscription = await _inscRepo.GetByIdAsync(request.InscriptionId, cancellationToken);

            if (inscription is null)
            {
                _logger.LogWarning("Aucune inscription trouvée avec l'identifiant : {InscriptionId}", request.InscriptionId);
                throw new KeyNotFoundException("Inscription non trouvée.");
            }
            // Liste des absences
            var absences = inscription.Presences
                .Where(p => p.Statut.Value ==StatutPresence.StatutEnum.Absent)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.Periode)
                .Select(p => new AbsenceDto(p.Date, p.Periode))
                .ToList();

            // Compte des périodes présentes
            var periodesPresentes = inscription.Presences
                .Count(p =>p.Statut.Value == StatutPresence.StatutEnum.Present);
            _logger.LogDebug("Nombre d'absences : {AbsCount}, Nombre de présences : {PresCount}",
                absences.Count, periodesPresentes);

            return new GetPresencesByInscriptionDto(absences, periodesPresentes);
        }
    }
}
