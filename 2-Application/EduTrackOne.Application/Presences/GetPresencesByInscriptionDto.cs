using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Presences
{
    public record GetPresencesByInscriptionDto(
        IReadOnlyList<AbsenceDto> Absences,
        int PeriodesPresentes
    );

    public record AbsenceDto(
        DateTime Date,
        int Periode
    );
}
