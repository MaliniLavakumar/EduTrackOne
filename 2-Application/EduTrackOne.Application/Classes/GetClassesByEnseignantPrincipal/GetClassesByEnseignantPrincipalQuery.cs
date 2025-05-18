using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal
{
    public record GetClassesByEnseignantPrincipalQuery(Guid PrincipalUserId)
    : IRequest<IEnumerable<ClassInfoDto>>;

}
