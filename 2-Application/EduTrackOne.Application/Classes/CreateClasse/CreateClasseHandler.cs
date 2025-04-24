using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Classes;
using MediatR;

namespace EduTrackOne.Application.Classes.Commands.CreateClasse
{
    public class CreateClasseHandler
        : IRequestHandler<CreateClasseCommand, Result<Guid>>
    {
        private readonly IClasseRepository _classeRepository;

        public CreateClasseHandler(IClasseRepository classeRepository)
        {
            _classeRepository = classeRepository;
        }

        public async Task<Result<Guid>> Handle(
            CreateClasseCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var classe = new Classe(
                    Guid.NewGuid(),
                    new NomClasse(command.NomClasse),
                    new AnneeScolaire(command.AnneeScolaire)
                );

                await _classeRepository.AddClasseAsync(classe);
                await _classeRepository.SaveChangesAsync();

                return Result<Guid>.Success(classe.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Erreur lors de la création de la classe : {ex.Message}");
            }
        }
    }
}
