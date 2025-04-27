using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.CreateClasse
{
    public class CreateClasseCommandValidator : AbstractValidator<CreateClasseCommand>
    {
        public CreateClasseCommandValidator()
        {
            RuleFor(x => x.NomClasse)
                .NotEmpty().WithMessage("Le nom de la classe ne peut pas être vide.")
                .Matches(@"^\d+[A-Za-z]/\d{2}-\d{4}$")
                    .WithMessage("Le nom de la classe doit suivre le format attendu (ex: 4P/01-2024).");

            // Validation de l'année scolaire
            RuleFor(x => x.AnneeScolaire)
                .NotEmpty().WithMessage("L'année scolaire ne peut pas être vide.")
                .Matches(@"^\d{4}-\d{4}$")
                    .WithMessage("L'année scolaire doit suivre le format attendu (ex: 2024-2025).");

            // Si un IdEnseignantPrincipal est fourni, il ne doit pas être Guid.Empty
            RuleFor(x => x.IdEnseignantPrincipal)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Si fourni, l'identifiant de l'enseignant principal doit être un GUID valide.");
        }
    }
}
