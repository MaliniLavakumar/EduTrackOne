using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.CreateClasse
{
    public class CreateClasseDto
    {
        public string NomClasse{ get; set; }
        public string AnneeScolaire { get; set; }
        public Guid? IdEnseignantPrincipal { get; set; } // Facultatif
    }
}

