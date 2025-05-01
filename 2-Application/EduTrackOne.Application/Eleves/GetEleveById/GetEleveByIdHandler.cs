using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Eleves;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.GetEleveById
{
    public class GetEleveByIdHandler : IRequestHandler<GetEleveByIdQuery, Result<EleveDto>>
    {
        private readonly IEleveRepository _repository;
        public GetEleveByIdHandler(IEleveRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<EleveDto>> Handle(GetEleveByIdQuery request, CancellationToken cancellationToken)
        {
            var eleve = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (eleve is null)
                return Result<EleveDto>.Failure("Élève non trouvé.");

            var dto = new EleveDto
            {
                Id = eleve.Id,
                Prenom = eleve.NomComplet.Prenom,
                Nom = eleve.NomComplet.Nom,
                DateNaissance = eleve.DateNaissance.Value,
                Sexe = eleve.Sexe.ToString(),
                Adresse = eleve.Adresse.ToString(),
                EmailParent = eleve.EmailParent.Value,
                Tel1 = eleve.Tel1.Value,
                Tel2 = eleve.Tel2?.Value,
                NoImmatricule = eleve.NoImmatricule

            };

            return Result<EleveDto>.Success(dto);
        }
    }
}
