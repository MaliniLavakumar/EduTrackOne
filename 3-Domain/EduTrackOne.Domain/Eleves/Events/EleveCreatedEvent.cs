using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Eleves.Events
{
    public sealed record EleveCreatedEvent(
       Guid EleveID,      
       string NoImmatricule    
   ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

}
