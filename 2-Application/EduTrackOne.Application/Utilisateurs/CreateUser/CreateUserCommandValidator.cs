using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.CreateUser
{
    public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand> 
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Dto.Identifiant).NotEmpty();
            RuleFor(x => x.Dto.MotDePasse).MinimumLength(6);
            RuleFor(x => x.Dto.Email).EmailAddress();
        }
    }
}
