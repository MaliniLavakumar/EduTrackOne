using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Notes.GetNotesByInscription
{
    public class GetNotesByInscriptionQueryValidator : AbstractValidator<GetNotesByInscriptionQuery>
    {
        public GetNotesByInscriptionQueryValidator()
        {
            RuleFor(x => x.InscriptionId).NotEmpty();
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.DateFin)
                .GreaterThanOrEqualTo(x => x.DateDebut)
                .When(x => x.DateDebut.HasValue && x.DateFin.HasValue)
                .WithMessage("La date de fin doit être postérieure à la date de début.");
        }
    }

}
