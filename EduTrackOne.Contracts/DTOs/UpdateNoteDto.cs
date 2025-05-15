using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    public record UpdateNoteDto(
     Guid InscriptionId,
     Guid NoteId,
     DateTime? DateExamen,
     Guid? MatiereId,
     double? Valeur,
     string? Commentaire
 );
}
