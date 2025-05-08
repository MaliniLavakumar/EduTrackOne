using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.RemoveInscription
{
    public class RemoveInscriptionCommandValidator : AbstractValidator<RemoveInscriptionCommand>
    {
        public RemoveInscriptionCommandValidator()
        {
            RuleFor(c => c.IdClasse).NotEmpty().WithMessage("ClasseId requis.");
            RuleFor(c => c.IdEleve).NotEmpty().WithMessage("EleveId requis.");

        }

    }
}
