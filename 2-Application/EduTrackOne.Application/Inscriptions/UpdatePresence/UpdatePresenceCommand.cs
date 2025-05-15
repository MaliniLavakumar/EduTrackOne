using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdatePresence
{
    public record UpdatePresenceCommand(
     Guid InscriptionId,
     Guid PresenceId,
     DateTime Date,
     int Periode,
     string Statut
 ) : IRequest<Result<Guid>>;
}
