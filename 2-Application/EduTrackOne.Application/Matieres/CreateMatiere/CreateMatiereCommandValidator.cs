using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Matieres.CreateMatiere
{
    public class CreateMatiereCommandValidator : AbstractValidator<CreateMatiereCommand>
    {
        public CreateMatiereCommandValidator()
        {
            RuleFor(x => x.Nom)
                .NotEmpty().WithMessage("Le nom de la matière est requis.")
                .MinimumLength(3).WithMessage("Le nom de la matière doit comporter au moins 3 caractères.")
                .MaximumLength(50).WithMessage("Le nom de la matière ne doit pas dépasser 50 caractères.");
        }
    }
}
