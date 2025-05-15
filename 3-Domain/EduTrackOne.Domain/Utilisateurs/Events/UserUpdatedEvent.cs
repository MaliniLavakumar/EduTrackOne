using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs.Events
{
    public record UserUpdatedEvent(Guid UserId, string NewIdentifiant, string NewEmail, DateTime OccurredOn): IDomainEvent;

}
