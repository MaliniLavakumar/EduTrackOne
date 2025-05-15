using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Presences
{
    public record GetPresencesByInscriptionQuery(Guid InscriptionId): IRequest<GetPresencesByInscriptionDto>;
   
}
