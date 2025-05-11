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

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public class UpdateEleveCommandHandler : IRequestHandler<UpdateEleveCommand, Result<Guid>>
    {
        private readonly IEleveRepository _eleveRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateEleveCommand> _validator;

        public UpdateEleveCommandHandler(IEleveRepository eleveRepository, IUnitOfWork unitOfWork, IValidator<UpdateEleveCommand> validator)
        {
            _eleveRepository = eleveRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(UpdateEleveCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }
            

            // 1. Récupérer l’élève
            var eleve = await _eleveRepository.GetByIdAsync(request.EleveId);
            if (eleve == null)
                return Result<Guid>.Failure("Élève introuvable.");

            // 2. Créer les nouveaux VO
            var nouvelleAdresse = new Adresse(request.Rue, request.CodePostal, request.Ville);
            var tel1 = new Telephone(request.Tel1);
            var tel2 = !string.IsNullOrWhiteSpace(request.Tel2) ? new Telephone(request.Tel2) : null;
            var emailParent = new Email(request.EmailParent);

            // 3. Mettre à jour les données et ajouter domain event
            eleve.ModifierCoordonnees(nouvelleAdresse, tel1, tel2, emailParent);

            // 4. Persister
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(eleve.Id);
        }
    }
}
