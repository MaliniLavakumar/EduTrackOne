using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Notes.GetNotesByInscription
{
    public record GetNotesByInscriptionDto(     
     Guid MatiereId,
     string MatiereNom,
     IEnumerable<NoteDto> Notes,
     double Moyenne
 );
    public record NoteDto(
    Guid NoteId,
    DateTime DateExamen,
    double? Valeur,
    bool EstAbsent
);
}
