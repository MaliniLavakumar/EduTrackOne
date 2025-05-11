using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record AddInscriptionDto(
        Guid ClasseId,
        string NoImmatricule,
        DateTime DateDebut,
        DateTime? DateFin
        );
}
