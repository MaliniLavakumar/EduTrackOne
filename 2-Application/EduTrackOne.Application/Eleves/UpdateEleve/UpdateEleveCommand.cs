using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public record UpdateEleveCommand(Guid EleveId,
        string Rue,
        string CodePostal,
        string Ville,
        string Tel1,
        string? Tel2,
        string EmailParent) :IRequest<Result<Guid>>;   
}