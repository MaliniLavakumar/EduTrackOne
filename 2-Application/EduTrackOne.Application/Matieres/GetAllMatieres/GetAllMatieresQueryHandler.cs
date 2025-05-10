using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Matieres;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Matieres.GetAllMatieres
{
    public class GetAllMatieresQueryHandler : IRequestHandler<GetAllMatieresQuery, Result<List<MatiereDto>>>
    {
        private readonly IMatiereRepository _repo;

        public GetAllMatieresQueryHandler(IMatiereRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<List<MatiereDto>>> Handle(
            GetAllMatieresQuery request,
            CancellationToken cancellationToken)
        {
            var list = await _repo.GetAllAsync(cancellationToken);
            var dtos = list
                .Select(m => new MatiereDto(m.Id, m.GetNom()))
                .ToList();
            return Result<List<MatiereDto>>.Success(dtos);
        }
    }

}

