using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse
{
    public class GetInscriptionsByClasseQueryValidator: AbstractValidator<GetInscriptionsByClasseQuery>
    {
        public GetInscriptionsByClasseQueryValidator()
        {
            RuleFor(x => x.ClasseId)
                .NotEmpty().WithMessage("Le Id de la classe est requis.");
        }
    }
}
