using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Eleves.Events
{
    public record EleveUpdatedEvent(
        Guid EleveID,
        Adresse NouvelleAdresse,
        Telephone Tel1,
        Telephone? Tel2,
        Email EmailParent
        ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
