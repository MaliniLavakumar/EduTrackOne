using EduTrackOne.Domain.Utilisateurs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Identifiant { get; }
        string? Role { get; }

        bool IsInRole(string role);
        bool IsInRole(RoleUtilisateur.Role admin);
    }
}
