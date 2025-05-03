using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public record UpdateEleveCommand(UpdateEleveDto Dto) :IRequest<Result<Guid>>;   
}