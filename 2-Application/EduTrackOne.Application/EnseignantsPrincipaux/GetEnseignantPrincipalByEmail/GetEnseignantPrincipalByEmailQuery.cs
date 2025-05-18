using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.GetEnseignantPrincipalByEmail
{
    public record GetEnseignantPrincipalByEmailQuery(string Email) : IRequest<Result<EnseignantPrincipalDto>>;

}
