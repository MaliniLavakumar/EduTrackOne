using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record UpdatePresenceDto(
    Guid InscriptionId,
    Guid PresenceId,
    DateTime Date,
    int Periode,
    string Statut
);
}
