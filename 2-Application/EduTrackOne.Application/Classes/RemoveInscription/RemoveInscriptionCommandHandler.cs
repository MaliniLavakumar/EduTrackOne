using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Inscriptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.RemoveInscription
{
    public class RemoveInscriptionCommandHandler : IRequestHandler<RemoveInscriptionCommand, Result<Guid>>
    {
        private readonly IClasseRepository _classeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RemoveInscriptionCommand> _validator;
        private readonly IInscriptionRepository _inscRepo;
        private readonly ILogger<RemoveInscriptionCommandHandler> _logger;

        public RemoveInscriptionCommandHandler(IClasseRepository classRepository, IUnitOfWork uow, IValidator<RemoveInscriptionCommand> validator, IInscriptionRepository inscRepo, ILogger<RemoveInscriptionCommandHandler> logger)
        {
            _classeRepository = classRepository;
            _unitOfWork = uow;
            _validator = validator;
            _inscRepo = inscRepo;
            _logger = logger;
        }
        public async Task<Result<Guid>> Handle(RemoveInscriptionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Début de suppression d’inscription pour l’élève {EleveId} de la classe {ClasseId}", request.IdEleve, request.IdClasse);
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(" | ", validation.Errors);
                _logger.LogWarning("Échec de validation pour la suppression d'inscription : {Errors}", errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }
            var classe = await _classeRepository.GetClasseByIdAsync(request.IdClasse, cancellationToken);
            if (classe is null)
            {
                _logger.LogWarning("Classe introuvable pour l’ID {ClasseId}", request.IdClasse);
                return Result<Guid>.Failure("Classe introuvable.");
            }

            var inscription = classe.TrouverInscriptionParEleve(request.IdEleve);
            if (inscription is null)
            {
                _logger.LogWarning("Inscription introuvable pour l’élève {EleveId} dans la classe {ClasseId}", request.IdEleve, request.IdClasse);
                return Result<Guid>.Failure("Inscription introuvable.");
            }

            var idInscription = inscription.Id;
            classe.SupprimerInscription(request.IdEleve);

            _logger.LogInformation("Inscription {InscriptionId} supprimée du domaine", idInscription);

            await _inscRepo.DeleteAsync(idInscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Inscription {InscriptionId} supprimée de la base de données avec succès", idInscription);
            return Result<Guid>.Success(idInscription);
        }
    }

}
