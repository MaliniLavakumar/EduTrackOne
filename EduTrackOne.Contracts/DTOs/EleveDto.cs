using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
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
