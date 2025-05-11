using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.AddInscription
{
    public class AddInscriptionCommandValidator : AbstractValidator<AddInscriptionCommand>
    {
        public AddInscriptionCommandValidator()
        {
            RuleFor(x => x.ClasseId)
                        .NotEmpty().WithMessage("L'ID de la classe est requis.");

            RuleFor(x => x.NoImmatricule)
                .NotEmpty().WithMessage("Le numéro d'immatriculation de l'élève est requis.");

            RuleFor(x => x.DateDebut)
                .NotEmpty().WithMessage("La date de début est requise.");

            RuleFor(x => x.DateFin)
                .GreaterThanOrEqualTo(x => x.DateDebut)
                .When(x => x.DateFin.HasValue)
                .WithMessage("La date de fin ne peut pas être antérieure à la date de début.");
        }
    }
}
