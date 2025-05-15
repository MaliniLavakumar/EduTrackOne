using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Presences
{
    public class GetPresencesByInscriptionQueryValidator: AbstractValidator<GetPresencesByInscriptionQuery>
    {
        public GetPresencesByInscriptionQueryValidator()
        {
            RuleFor(x => x.InscriptionId).NotEmpty();
        }
    }
}
