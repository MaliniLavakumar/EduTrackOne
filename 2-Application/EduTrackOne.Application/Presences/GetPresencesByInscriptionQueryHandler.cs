using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
using MediatR;
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

        public GetPresencesByInscriptionQueryHandler(IInscriptionRepository inscRepo)
            => _inscRepo = inscRepo;

        public async Task<GetPresencesByInscriptionDto> Handle(
            GetPresencesByInscriptionQuery request,
            CancellationToken cancellationToken)
        {
            var inscription = await _inscRepo
                .GetByIdAsync(request.InscriptionId, cancellationToken)
                ?? throw new KeyNotFoundException("Inscription non trouvée.");

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

            return new GetPresencesByInscriptionDto(absences, periodesPresentes);
        }
    }
}
