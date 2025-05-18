using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.GetAllEnseignantsPrincipaux
{
    public class GetAllEnseignantsPrincipauxQuery : IRequest<Result<List<EnseignantPrincipalDto>>> { }
}

