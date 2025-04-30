using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using FluentValidation;
using MediatR;
using System;

namespace EduTrackOne.Application.Eleves.CreateEleve
{
    public class CreateEleveHandler : IRequestHandler<CreateEleveCommand, Result<Guid>>
    {
        private readonly IEleveRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateEleveCommand> _validator;


        public CreateEleveHandler(
           IEleveRepository repo,
           IUnitOfWork uow,
           IValidator<CreateEleveCommand> validator)
        {
            _repo = repo;
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(CreateEleveCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }
            if (await _repo.ExistsByNoImmatriculeAsync(request.NoImmatricule, cancellationToken))
            {
                return Result<Guid>.Failure("Un élève avec ce numéro d'immatricule existe déjà.");
            }
            var nomComplet = new NomComplet(request.Prenom, request.Nom);
            var dateNaissance = new DateNaissance(request.DateNaissance);
            var sexe = new Sexe(Enum.Parse<Sexe.SexeType>(request.Sexe, true));
            var adresse = new Adresse(request.Rue, request.CodePostal, request.Ville);
            var emailParent = new Email(request.EmailParent);
            var tel1 = new Telephone(request.Tel1);
            Telephone? tel2 = string.IsNullOrWhiteSpace(request.Tel2)
                                ? null
                                : new Telephone(request.Tel2);

            var eleve = new Eleve(
                            Guid.NewGuid(),
                            nomComplet,
                            dateNaissance,
                            sexe,
                            adresse,
                            emailParent,
                            tel1,
                            tel2,
                            request.NoImmatricule
                        );
            await _repo.AddAsync(eleve);
            await _uow.SaveChangesAsync(cancellationToken);
           
            return Result<Guid>.Success(eleve.Id);
        }
    }
}
