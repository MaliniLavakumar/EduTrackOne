using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse
{
    public record GetInscriptionsByClasseQuery(Guid ClasseId): IRequest<Result<List<GetInscriptionsByClasseDto>>>;
    
}
