using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions.Events
{
    public record PresenceAddedEvent(
        Guid PresenceId,
        Guid EleveId,
        Guid InscriptionId,
        Guid ClasseId,
        DateTime Date,
        int Periode,
        string Statut
        ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

}
