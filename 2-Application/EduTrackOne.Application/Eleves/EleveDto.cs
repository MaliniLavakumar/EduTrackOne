using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves
{
    public record EleveDto
    (
        Guid Id,
        string Prenom,
        string Nom,
        DateTime DateNaissance,
        string Sexe,
        string Adresse,
        string EmailParent,
        string Tel1,
        string? Tel2,
        string NoImmatricule
        );

}
