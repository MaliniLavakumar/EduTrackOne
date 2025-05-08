using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.AddInscription
{
    public record AddInscriptionDto(
        Guid ClasseId,
        string NoImmatricule,
        DateTime DateDebut,
        DateTime? DateFin
        );
}
