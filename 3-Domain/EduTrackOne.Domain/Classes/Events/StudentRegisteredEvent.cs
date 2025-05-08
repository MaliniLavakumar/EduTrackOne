using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Classes.Events
{
    public record StudentRegisteredEvent(
        Guid ClasseId,
        Guid EleveId,
        DateInscriptionPeriode Periode

      ): IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
