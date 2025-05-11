using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record CreateClasseDto(

         string NomClasse,
         string AnneeScolaire,
         Guid? IdEnseignantPrincipal
    );
}

