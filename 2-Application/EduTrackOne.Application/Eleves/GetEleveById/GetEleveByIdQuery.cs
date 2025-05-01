using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.GetEleveById
{
    public record GetEleveByIdQuery(Guid Id) : IRequest<Result<EleveDto>>;

}
