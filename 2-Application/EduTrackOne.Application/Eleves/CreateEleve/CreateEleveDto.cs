using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.CreateEleve
{
    public class CreateEleveDto
    {
        public string Prenom { get; set; } = default!;
        public string Nom { get; set; } = default!;
        public DateTime DateNaissance { get; set; }
        public string Sexe { get; set; } = default!;
        public string Rue { get; set; } = default!;
        public string CodePostal { get; set; } = default!;
        public string Ville { get; set; } = default!;
        public string EmailParent { get; set; } = default!;
        public string Tel1 { get; set; } = default!;
        public string? Tel2 { get; set; }
        public string NoImmatricule { get; set; } = default!;
    }
}


