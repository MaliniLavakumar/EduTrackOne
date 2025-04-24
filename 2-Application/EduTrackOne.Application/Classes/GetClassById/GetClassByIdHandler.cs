using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Classes;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.Queries.GetClasseById
{
    public class GetClasseByIdHandler : IRequestHandler<GetClasseByIdQuery, Result<ClasseDto>>
    {
        private readonly IClasseRepository _classeRepository;

        public GetClasseByIdHandler(IClasseRepository classeRepository)
        {
            _classeRepository = classeRepository;
        }

        public async Task<Result<ClasseDto>> Handle(GetClasseByIdQuery request, CancellationToken cancellationToken)
        {
            var classe = await _classeRepository.GetClasseByIdAsync(request.Id);
            if (classe == null)
                return Result<ClasseDto>.Failure("Classe introuvable.");

            var dto = new ClasseDto
            {
                Id = classe.Id,
                NomClasse = classe.Nom.Value,
                AnneeScolaire = classe.AnneeScolaire.Value
            };

            return Result<ClasseDto>.Success(dto);
        }
    }
}
