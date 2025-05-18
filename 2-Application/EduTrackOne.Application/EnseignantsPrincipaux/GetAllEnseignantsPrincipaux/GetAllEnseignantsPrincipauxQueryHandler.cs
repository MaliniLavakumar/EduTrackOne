using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.GetAllEnseignantsPrincipaux
{

    public class GetAllEnseignantsPrincipauxQueryHandler
        : IRequestHandler<GetAllEnseignantsPrincipauxQuery, Result<List<EnseignantPrincipalDto>>>
    {
        private readonly IEnseignantPrincipalRepository _repo;

        public GetAllEnseignantsPrincipauxQueryHandler(IEnseignantPrincipalRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<List<EnseignantPrincipalDto>>> Handle(
            GetAllEnseignantsPrincipauxQuery request,
            CancellationToken cancellationToken)
        {
            var enseignants = await _repo.GetAllAsync();
            var dtos = enseignants
                .Select(ep => new EnseignantPrincipalDto(
                    ep.Id,
                    ep.NomComplet.Prenom,
                    ep.NomComplet.Nom,
                    ep.Email.Value))
                .ToList();

            return Result<List<EnseignantPrincipalDto>>.Success(dtos);
        }
    }
}
