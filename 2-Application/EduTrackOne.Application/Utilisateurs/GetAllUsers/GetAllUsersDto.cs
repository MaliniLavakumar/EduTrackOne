using EduTrackOne.Domain.Utilisateurs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.GetAllUsers
{
   public record GetAllUsersDto(
        Guid Id,
        string Identifiant,
        RoleUtilisateur.Role Role,
        StatutUtilisateur.StatutEnum Statut,
        string Email
    );
    
}
