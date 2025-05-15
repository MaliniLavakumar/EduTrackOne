using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions.Events
{
    public record NoteUpdatedEvent(
        Guid InscriptionId,
        Guid NoteId,
        Guid EleveId,
        Guid ClasseId,
        Guid MatiereId,
        DateTime NouvelleDateExamen,
        double? NouvelleValeur,
        bool EstAbsent
    ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
