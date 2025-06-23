using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record LoginDto(
           [Required(ErrorMessage = "Le champ Identifiant est requis.")]
        string Identifiant,

           [Required(ErrorMessage = "Le champ Mot de passe est requis.")]
        [DataType(DataType.Password)]
        string MotDePasse
       );
}
