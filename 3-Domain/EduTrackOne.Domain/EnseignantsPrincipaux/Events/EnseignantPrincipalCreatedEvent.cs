using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.EnseignantsPrincipaux.Events
{
    public sealed record EnseignantPrincipalCreatedEvent(
        Guid EnseignantPrincipalId,
        string NomComplet,
        string Email
    ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }


}
