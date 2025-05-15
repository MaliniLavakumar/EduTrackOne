using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record UpdateUserDto(
        Guid Id,
        string Identifiant,
        UserRoleDto Role,
        UserStatusDto Statut,
        string Email
    );
   
}
