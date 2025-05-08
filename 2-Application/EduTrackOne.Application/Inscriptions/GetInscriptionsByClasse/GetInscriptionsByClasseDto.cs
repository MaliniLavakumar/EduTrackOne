using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse
{
    public record GetInscriptionsByClasseDto(
        Guid InscriptionId,
        Guid EleveId,
        string EleveNom
        );
   
}
