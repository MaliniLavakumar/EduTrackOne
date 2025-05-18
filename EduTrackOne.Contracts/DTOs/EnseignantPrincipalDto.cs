using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record EnseignantPrincipalDto(
        Guid Id,
        string Prenom,
        string Nom,
        string Email
    );
}
