using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.EnseignantsPrincipaux.GetEnseignantPrincipalByEmail
{
    public class GetEnseignantPrincipalByEmailQueryHandler
     : IRequestHandler<GetEnseignantPrincipalByEmailQuery, Result<EnseignantPrincipalDto>>
    {
        private readonly IEnseignantPrincipalRepository _repo;
        public GetEnseignantPrincipalByEmailQueryHandler(IEnseignantPrincipalRepository repo) => _repo = repo;

        public async Task<Result<EnseignantPrincipalDto>> Handle(
            GetEnseignantPrincipalByEmailQuery request,
            CancellationToken ct)
        {
            var list = await _repo.GetByEmailAsync(request.Email, ct);
            var ep = list.FirstOrDefault();
            return ep is null
                ? Result<EnseignantPrincipalDto>.Failure("Enseignant non trouvé.")
                : Result<EnseignantPrincipalDto>.Success(
                    new EnseignantPrincipalDto(ep.Id, ep.NomComplet.Prenom, ep.NomComplet.Nom, ep.Email.Value)
                );
        }
    }
}
