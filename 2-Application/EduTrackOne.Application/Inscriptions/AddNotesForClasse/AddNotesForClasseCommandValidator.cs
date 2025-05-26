using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.AddNotesForClasse
{
    public class AddNotesForClasseCommandValidator: AbstractValidator<AddNotesForClasseCommand>
    {
        public AddNotesForClasseCommandValidator()
        {
            RuleFor(x => x.ClasseId)
                .NotEmpty().WithMessage("L'ID de la classe est requis.");

            RuleFor(x => x.DateExamen)
                .NotEmpty().WithMessage("La date de l'examen est requise.");

            RuleFor(x => x.Notes)
                .NotEmpty().WithMessage("La liste des notes ne peut pas être vide.");

            RuleForEach(x => x.Notes).ChildRules(notes =>
            {
                notes.RuleFor(n => n.EleveId)
                     .NotEmpty().WithMessage("Chaque note doit avoir un EleveId.");

                notes.RuleFor(n => n.MatiereId)
                     .NotEmpty().WithMessage("Chaque note doit avoir un MatiereId.");

                notes.RuleFor(n => n.Valeur)
                     .Must(v => v!= -1 &&(v == null || (v >= 1 && v <= 6 && v % 0.5 == 0)))
                     .WithMessage("Vous devez choisir 'Absent' ou une valeur entre 1 et 6 par pas de 0.5.");
            });
        }

    }
}
