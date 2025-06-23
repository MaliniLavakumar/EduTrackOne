using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record CreateMatiereDto(
        [property: Required(ErrorMessage = "Le nom de la matière est requis.")]
        [property: StringLength(100, ErrorMessage = "Le nom de la matière ne peut pas dépasser 100 caractères.")]
     string NomMatiere);
    
}
