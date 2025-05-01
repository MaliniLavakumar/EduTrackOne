using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves
{
    public class EleveDto
    {
        public Guid Id { get; set; }
        public string Prenom { get; set; } = default!;
        public string Nom { get; set; } = default!;
        public DateTime DateNaissance { get; set; }
        public string Sexe { get; set; } = default!;
        public string Adresse { get; set; } = default!;
        public string EmailParent { get; set; } = default!;
        public string Tel1 { get; set; } = default!;
        public string? Tel2 { get; set; }
        public string NoImmatricule { get; set; } = default!;
    }
}
