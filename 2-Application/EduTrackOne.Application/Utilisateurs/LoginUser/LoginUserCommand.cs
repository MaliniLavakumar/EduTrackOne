using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Contracts.DTOs.EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.LoginUser
{
    public record LoginUserCommand(LoginDto Dto) : IRequest<LoginUserResponseDto>;
}