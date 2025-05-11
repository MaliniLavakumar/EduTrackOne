using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record PresenceEleveDto(
    Guid EleveId,
    string Statut // "Present" ou "Absent"
);

    public record AddPresencesForClasseDto(
    Guid ClasseId,
    DateTime Date,
    int Periode,
    List<PresenceEleveDto> Presences  // ceux présents
);
    
}
