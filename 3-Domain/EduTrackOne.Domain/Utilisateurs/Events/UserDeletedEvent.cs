using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs.Events
{
    public record UserDeletedEvent(Guid UserId, string Identifiant) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    }
}
