using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using MediatR;

namespace EduTrackOne.Application.Classes.CreateClasse
{
    public class CreateClasseHandler
        : IRequestHandler<CreateClasseCommand, Result<Guid>>
    {
        private readonly IClasseRepository _classeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateClasseHandler(IClasseRepository classeRepository, IUnitOfWork unitOfWork)
        {
            _classeRepository = classeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateClasseCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var nomVo = new NomClasse(command.NomClasse);
                var anneeVo = new AnneeScolaire(command.AnneeScolaire);

                // Création de l'agrégat
                var classe = new Classe(Guid.NewGuid(), nomVo, anneeVo);

                // Si on a un enseignant principal, on l'assigne
                if (command.IdEnseignantPrincipal.HasValue)
                    classe.AssignerEnseignantPrincipal(command.IdEnseignantPrincipal.Value);

                // Persister
                await _classeRepository.AddClasseAsync(classe);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(classe.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Erreur lors de la création de la classe : {ex.Message}");
            }
        }
    }
}
