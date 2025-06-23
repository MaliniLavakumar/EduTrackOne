using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Dto.AncienMotDePasse)
                .NotEmpty().WithMessage("L’ancien mot de passe est requis.");

            RuleFor(x => x.Dto.NouveauMotDePasse)
                .NotEmpty().WithMessage("Le nouveau mot de passe est requis.")
                .MinimumLength(6).WithMessage("Le nouveau mot de passe doit contenir au moins 6 caractères.")
                .Matches("[A-Z]").WithMessage("Le mot de passe doit contenir au moins une majuscule.")
                .Matches("[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.");
        }

    }
}
