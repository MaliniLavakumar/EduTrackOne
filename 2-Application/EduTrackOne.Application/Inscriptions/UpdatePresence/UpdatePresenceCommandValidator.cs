using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdatePresence
{
    public class UpdatePresenceCommandValidator : AbstractValidator<UpdatePresenceCommand>
    {
        public UpdatePresenceCommandValidator()
        {
            RuleFor(x => x.InscriptionId).NotEmpty();
            RuleFor(x => x.PresenceId).NotEmpty();
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Periode).GreaterThan(0);
            RuleFor(x => x.Statut)
                .Must(s => s.Equals("Present", StringComparison.OrdinalIgnoreCase)
                        || s.Equals("Absent", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Statut must be 'Present' or 'Absent'.");
        }
    }
    
}
