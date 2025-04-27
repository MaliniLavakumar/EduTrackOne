using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal
{
    public class CreateEnseignantPrincipalDto
    {
        public string Prenom { get; set; } = default!;
        public string Nom { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
