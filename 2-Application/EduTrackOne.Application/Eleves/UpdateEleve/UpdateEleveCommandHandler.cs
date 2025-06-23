using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves.Events;
using EduTrackOne.Domain.Eleves;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public class UpdateEleveCommandHandler : IRequestHandler<UpdateEleveCommand, Result<Guid>>
    {
        private readonly IEleveRepository _eleveRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateEleveCommand> _validator;
        private readonly ILogger<UpdateEleveCommandHandler> _logger;
        public UpdateEleveCommandHandler(IEleveRepository eleveRepository, IUnitOfWork unitOfWork, IValidator<UpdateEleveCommand> validator, ILogger<UpdateEleveCommandHandler> logger)
        {
            _eleveRepository = eleveRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(UpdateEleveCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Début de la mise à jour des coordonnées de l'élève {EleveId}", request.EleveId);
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors);
                _logger.LogWarning("Échec de validation pour l'élève {EleveId} : {Errors}", request.EleveId, errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }
            

            // 1. Récupérer l’élève
            var eleve = await _eleveRepository.GetByIdAsync(request.EleveId);
            if (eleve == null)
            {
                _logger.LogWarning("Élève introuvable avec l’ID {EleveId}", request.EleveId);
                return Result<Guid>.Failure("Élève introuvable.");
            }
            // 2. Créer les nouveaux VO
            var nouvelleAdresse = new Adresse(request.Rue, request.CodePostal, request.Ville);
            var tel1 = new Telephone(request.Tel1);
            var tel2 = !string.IsNullOrWhiteSpace(request.Tel2) ? new Telephone(request.Tel2) : null;
            var emailParent = new Email(request.EmailParent);

            // 3. Mettre à jour les données et ajouter domain event
            eleve.ModifierCoordonnees(nouvelleAdresse, tel1, tel2, emailParent);
            _logger.LogInformation("Coordonnées de l'élève {EleveId} mises à jour avec succès", eleve.Id);

            // 4. Persister
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Modifications sauvegardées pour l'élève {EleveId}", eleve.Id); 
            return Result<Guid>.Success(eleve.Id);
        }
    }
}
