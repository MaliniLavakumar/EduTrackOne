using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.CreateClasse
{
    public class CreateClasseDtoValidator : AbstractValidator<CreateClasseDto>
    {
        public CreateClasseDtoValidator()
        {
            RuleFor(x => x.NomClasse)
                .NotEmpty().WithMessage("Le nom de la classe ne peut pas être vide.")
                .Matches(@"^\d+[A-Za-z]/\d{2}-\d{4}$").WithMessage("Le nom de la classe doit suivre le format attendu (ex: 4P/01-2024).");

            RuleFor(x => x.AnneeScolaire)
                .NotEmpty().WithMessage("L'année scolaire ne peut pas être vide.")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("L'année scolaire doit suivre le format attendu (ex: 2024-2025).");
        }
    }
}
