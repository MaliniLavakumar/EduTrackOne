using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs.Events
{
    public record UserLoggedInEvent(Guid UserId, DateTime OccurredOn) : IDomainEvent;
}
