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
            RuleFor(x => x.Dto.ClasseId)
                        .NotEmpty().WithMessage("L'ID de la classe est requis.");

            RuleFor(x => x.Dto.NoImmatricule)
                .NotEmpty().WithMessage("Le numéro d'immatriculation de l'élève est requis.");

            RuleFor(x => x.Dto.DateDebut)
                .NotEmpty().WithMessage("La date de début est requise.");

            RuleFor(x => x.Dto.DateFin)
                .GreaterThanOrEqualTo(x => x.Dto.DateDebut)
                .When(x => x.Dto.DateFin.HasValue)
                .WithMessage("La date de fin ne peut pas être antérieure à la date de début.");
        }
    }
}
