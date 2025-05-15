using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.LoginUser
{
    public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Dto.Identifiant)
                .NotEmpty().WithMessage("L'identifiant est requis.");

            RuleFor(x => x.Dto.MotDePasse)
                .NotEmpty().WithMessage("Le mot de passe est requis.");
        }
    }
}
