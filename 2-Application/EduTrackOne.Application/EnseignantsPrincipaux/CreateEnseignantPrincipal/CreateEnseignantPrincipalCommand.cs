using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal
{
    public record CreateEnseignantPrincipalCommand(
         string Prenom,
         string Nom,
         string Email
     ) : IRequest<Result<Guid>>;
}
