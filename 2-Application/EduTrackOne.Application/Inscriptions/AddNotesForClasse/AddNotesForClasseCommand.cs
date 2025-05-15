using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.AddNotesForClasse
{
    public record AddNotesForClasseCommand(
        Guid ClasseId,
        DateTime DateExamen,
        List<NoteForEleveDto> Notes
    ) : IRequest<Result<int>>;
   
}
