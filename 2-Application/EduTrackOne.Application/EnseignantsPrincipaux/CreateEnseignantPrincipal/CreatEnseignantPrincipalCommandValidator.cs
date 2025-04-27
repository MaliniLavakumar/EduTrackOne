using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal
{
    public class CreateEnseignantPrincipalCommandValidator
        : AbstractValidator<CreateEnseignantPrincipalCommand>
    {
        public CreateEnseignantPrincipalCommandValidator()
        {
            RuleFor(x => x.Prenom)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .Length(2, 100);

            RuleFor(x => x.Nom)
                .NotEmpty().WithMessage("Le nom est requis.")
                .Length(2, 100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Format d'email invalide.");
        }
    }
}
