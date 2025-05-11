    using EduTrackOne.Application.Common;
    using EduTrackOne.Domain.Abstractions;
    using EduTrackOne.Domain.Classes;
    using EduTrackOne.Domain.Eleves;
    using EduTrackOne.Domain.Inscriptions;
    using FluentValidation;
    using MediatR;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    namespace EduTrackOne.Application.Classes.AddInscription
    {
        public class AddInscriptionCommandHandler : IRequestHandler<AddInscriptionCommand, Result<Guid>>
        {
            private readonly IClasseRepository _classeRepo;
            private readonly IEleveRepository _eleveRepo;
            private readonly IInscriptionRepository _inscRepo;
            private readonly IUnitOfWork _uow;
            private readonly IValidator<AddInscriptionCommand> _validator;

            public AddInscriptionCommandHandler(
                IClasseRepository classeRepo,
                IEleveRepository eleveRepo,
                IInscriptionRepository inscRepo,
                IUnitOfWork uow,
                IValidator<AddInscriptionCommand> validator

              )
            {
                _classeRepo = classeRepo;
                _eleveRepo = eleveRepo;
                _inscRepo =inscRepo;
                _uow = uow;
                _validator = validator;
            }
            public async Task<Result<Guid>> Handle(AddInscriptionCommand request, CancellationToken cancellationToken)
            {
                // 1. Validation manuelle
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result<Guid>.Failure(errors);
                }

                

                // 2. Charger la classe
                var classe = await _classeRepo.GetClasseByIdAsync(request.ClasseId, cancellationToken);
                if (classe is null)
                    return Result<Guid>.Failure("Classe introuvable.");

                // 3. Rechercher l'élève par immatriculation
                var eleve = await _eleveRepo.GetByNoImmatriculeAsync(request.NoImmatricule, cancellationToken);
                if (eleve == null)
                    return Result<Guid>.Failure("Aucun élève avec ce numéro d'immatriculation.");

                // 4. Vérifier qu'il n'est pas déjà inscrit
                if (classe.Inscriptions.Any(i => i.IdEleve == eleve.Id))
                    return Result<Guid>.Failure("Cet élève est déjà inscrit dans la classe.");

                // 5. Construire et valider la période
                var periodeResult = ConstruirePeriode(request.DateDebut, request.DateFin);
                if (!periodeResult.IsSuccess)
                    return Result<Guid>.Failure($"Période invalide : {periodeResult.Error}");

                var periode = periodeResult.Value;


                // 6. Inscrire l'élève
                Guid inscriptionId;
                try
                {
                    inscriptionId = classe.InscrireEleve(eleve.Id, periode);
                }
                catch (Exception ex)
                {
                    return Result<Guid>.Failure(ex.Message);
                }

                // 7. Persister
           
                var nouvelleInsc = classe.Inscriptions.Single(i=> i.Id == inscriptionId);
                await _inscRepo.AddAsync(nouvelleInsc, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(inscriptionId);
            }


            private Result<DateInscriptionPeriode> ConstruirePeriode(DateTime debut, DateTime? fin)
            {
                try
                {
                    var periode = new DateInscriptionPeriode(debut, fin);
                    return Result<DateInscriptionPeriode>.Success(periode);
                }
                catch (ArgumentException ex)
                {
                    return Result<DateInscriptionPeriode>.Failure(ex.Message);
                }
            }
        }
    }




