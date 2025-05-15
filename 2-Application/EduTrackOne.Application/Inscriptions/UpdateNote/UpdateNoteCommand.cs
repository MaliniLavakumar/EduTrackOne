using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdateNote
{
    public record UpdateNoteCommand(
     Guid InscriptionId,
     Guid NoteId,
     DateTime NouvelleDateExamen,
     Guid NouvelleMatiereId,
     double? NouvelleValeur,
     string? NouveauCommentaire
 ) : IRequest<Result<Guid>>;
}
