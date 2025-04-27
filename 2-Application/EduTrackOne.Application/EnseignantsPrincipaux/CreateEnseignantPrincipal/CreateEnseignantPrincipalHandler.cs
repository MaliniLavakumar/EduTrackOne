using EduTrackOne.Application.Common;
using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using MediatR;

namespace EduTrackOne.Application.Enseignants.Commands.CreateEnseignantPrincipal
{
    public class CreateEnseignantPrincipalHandler : IRequestHandler<CreateEnseignantPrincipalCommand, Result<Guid>>
    {
        private readonly IEnseignantPrincipalRepository _enseignantPrincipalRepository;

        public CreateEnseignantPrincipalHandler(IEnseignantPrincipalRepository enseignantPrincipalRepository)
        {
            _enseignantPrincipalRepository = enseignantPrincipalRepository;
        }

        public async Task<Result<Guid>> Handle(CreateEnseignantPrincipalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var nomComplet = new NomComplet(request.Prenom, request.Nom);
                var enseignantPrincipal = new EnseignantPrincipal(
                    Guid.NewGuid(),
                    nomComplet,                  
                    new Email(request.Email)
                );

                await _enseignantPrincipalRepository.AddAsync(enseignantPrincipal);
                await _enseignantPrincipalRepository.SaveChangesAsync();

                return Result<Guid>.Success(enseignantPrincipal.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Erreur lors de la création de l'enseignant : {ex.Message}");
            }
        }
    }
}
