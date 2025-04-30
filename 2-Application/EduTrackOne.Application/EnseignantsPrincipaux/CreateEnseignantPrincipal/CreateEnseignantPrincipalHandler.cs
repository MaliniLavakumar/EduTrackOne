using EduTrackOne.Application.Common;
using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using FluentValidation;
using MediatR;

namespace EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal
{
    public class CreateEnseignantPrincipalHandler
        : IRequestHandler<CreateEnseignantPrincipalCommand, Result<Guid>>
    {
        private readonly IEnseignantPrincipalRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateEnseignantPrincipalCommand> _validator;

        public CreateEnseignantPrincipalHandler(
            IEnseignantPrincipalRepository repo,
            IUnitOfWork uow,
            IValidator<CreateEnseignantPrincipalCommand> validator)
        {
            _repo = repo;
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(
            CreateEnseignantPrincipalCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validation de la commande
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }

            // 2. Création des Value Objects
            var nomComplet = new NomComplet(request.Prenom, request.Nom);
            var emailVo = new Email(request.Email);

            // 3. Création de l'agrégat
            //    Le constructeur lève automatiquement EnseignantPrincipalCreatedEvent
            var enseignant = new EnseignantPrincipal(Guid.NewGuid(), nomComplet, emailVo);

            // 4. Persistance
            await _repo.AddAsync(enseignant);
            await _uow.SaveChangesAsync(cancellationToken);

            // 5. Retour du résultat avec l'ID de l'enseignant créé
            return Result<Guid>.Success(enseignant.Id);
        }
    }
}
