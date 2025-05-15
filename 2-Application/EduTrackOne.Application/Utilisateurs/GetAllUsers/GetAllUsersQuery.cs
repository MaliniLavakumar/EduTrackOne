using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.GetAllUsers
{
    public record GetAllUsersQuery : IRequest<IEnumerable<GetAllUsersDto>>;
}

