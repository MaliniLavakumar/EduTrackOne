using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Classes;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.GetClasseById
{
    public class GetClasseByIdHandler : IRequestHandler<GetClasseByIdQuery, Result<ClasseDto>>
    {
        private readonly IClasseRepository _classeRepository;
        private readonly ILogger<GetClasseByIdHandler> _logger;
        public GetClasseByIdHandler(IClasseRepository classeRepository, ILogger<GetClasseByIdHandler> logger)
        {
            _classeRepository = classeRepository;
            _logger = logger;
        }

        public async Task<Result<ClasseDto>> Handle(GetClasseByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Recherche de la classe avec l’ID {ClasseId}", request.Id);

            var classe = await _classeRepository.GetClasseByIdAsync(request.Id);
            if (classe == null)
            {
                _logger.LogWarning("Classe introuvable pour l’ID {ClasseId}", request.Id);
                return Result<ClasseDto>.Failure("Classe introuvable.");
            }

            _logger.LogInformation("Classe trouvée : {ClasseNom} ({ClasseId})", classe.Nom.Value, classe.Id);

            return Result<ClasseDto>.Success(new ClasseDto
            (
                classe.Id,
                classe.Nom.Value,
                classe.AnneeScolaire.Value,
                classe.IdEnseignantPrincipal
            ));


        }
    }
}
