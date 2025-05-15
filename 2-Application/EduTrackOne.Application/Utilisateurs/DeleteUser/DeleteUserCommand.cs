using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.DeleteUser
{
    public record DeleteUserCommand(Guid UserId) : IRequest<Unit>;
}
