using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions.Events
{
    public record PresenceUpdatedEvent(
        Guid InscriptionId,
        Guid PresenceId,
        Guid EleveId,
        Guid ClasseId,
        DateTime Date,
        int Periode,
        string NouveauStatut
    ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
