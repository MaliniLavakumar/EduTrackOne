using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.AddPresencesForClasse
{
    public class AddPresencesForClasseCommandValidator: AbstractValidator<AddPresencesForClasseCommand>
    {
        public AddPresencesForClasseCommandValidator()
        {
            RuleFor(x => x.ClasseId)
                .NotEmpty().WithMessage("L'ID de la classe est requis.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La date de présence est requise.");

            RuleFor(x => x.Periode)
                .NotEmpty().WithMessage("La période est requise.")
                .InclusiveBetween(1, 8)
                .WithMessage("La période doit être comprise entre 1 et 8.");


            RuleFor(x => x.Presences)
            .NotEmpty().WithMessage("La liste des présences ne peut pas être vide.")
            .Must(list => list.Select(p => p.EleveId).Distinct().Count() == list.Count)
            .WithMessage("Les identifiants des élèves doivent être uniques.");

            RuleForEach(x => x.Presences).ChildRules(presence =>
            {
                presence.RuleFor(p => p.EleveId)
                    .NotEmpty().WithMessage("L'ID de l'élève est requis.");

                presence.RuleFor(p => p.Statut)
                    .Must(s => s == "Present" || s == "Absent")
                    .WithMessage("Le statut doit être 'Present' ou 'Absent'.");
            });
        }

        }
}
