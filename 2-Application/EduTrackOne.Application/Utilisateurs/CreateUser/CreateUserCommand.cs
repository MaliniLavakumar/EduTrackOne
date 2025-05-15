using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.CreateUser
{
    public record CreateUserCommand(CreateUserDto Dto) : IRequest<Guid>;
}
