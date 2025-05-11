using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public class UpdateEleveCommandValidator : AbstractValidator<UpdateEleveCommand>
    {
        public UpdateEleveCommandValidator()
        {
            RuleFor(x => x.EleveId).NotEmpty();

            RuleFor(x => x.Rue)
                .NotEmpty().WithMessage("La rue est obligatoire.")
                .MaximumLength(100);

            RuleFor(x => x.CodePostal)
                .NotEmpty().WithMessage("Le code postal est obligatoire.")
                .MaximumLength(10);

            RuleFor(x => x.Ville)
                .NotEmpty().WithMessage("La ville est obligatoire.")
                .MaximumLength(50);

            RuleFor(x => x.Tel1)
                .NotEmpty().WithMessage("Le numéro principal est obligatoire.")
                .Matches(@"^\+41\d{9}$").WithMessage("Le numéro doit commencer par +41 et contenir 9 chiffres après.");

            RuleFor(x => x.Tel2)
                .Matches(@"^\+41\d{9}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Tel2))
                .WithMessage("Le deuxième numéro (s'il est présent) doit aussi être un numéro suisse valide.");

            RuleFor(x => x.EmailParent)
                .NotEmpty().WithMessage("L'adresse email du parent est obligatoire.")
                .EmailAddress().WithMessage("L'email n'a pas un format valide.");
        }
    }
}

