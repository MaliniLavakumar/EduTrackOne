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
            (
                eleve.Id,
                eleve.NomComplet.Prenom,
                eleve.NomComplet.Nom,
                eleve.DateNaissance.Value,
                eleve.Sexe.ToString(),
                eleve.Adresse.ToString(),
                eleve.EmailParent.Value,
                eleve.Tel1.Value,
                eleve.Tel2?.Value,
                eleve.NoImmatricule
                );



            return Result<EleveDto>.Success(dto);
        }
    }
}
