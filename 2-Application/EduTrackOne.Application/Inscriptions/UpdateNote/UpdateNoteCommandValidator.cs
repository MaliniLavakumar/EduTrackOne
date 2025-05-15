using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdateNote
{
    public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
    {
        public UpdateNoteCommandValidator()
        {
            RuleFor(x => x.InscriptionId).NotEmpty();
            RuleFor(x => x.NoteId).NotEmpty();
            RuleFor(x => x.NouvelleDateExamen).NotEmpty();
            RuleFor(x => x.NouvelleMatiereId).NotEmpty();
            RuleFor(x => x.NouvelleValeur)
                .Must(v => v == null || (v >= 1 && v <= 6))
                .WithMessage("La note doit être entre 1 et 6 ou null pour absent.");
        }
    }
}
