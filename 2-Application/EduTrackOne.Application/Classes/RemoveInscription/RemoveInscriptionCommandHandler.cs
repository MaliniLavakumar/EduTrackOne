using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Inscriptions;
using FluentValidation;
using MediatR;
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

        public RemoveInscriptionCommandHandler(IClasseRepository classRepository, IUnitOfWork uow, IValidator<RemoveInscriptionCommand> validator, IInscriptionRepository inscRepo)
        {
            _classeRepository = classRepository;
            _unitOfWork = uow;
            _validator = validator;
            _inscRepo = inscRepo;
        }
        public async Task<Result<Guid>> Handle(RemoveInscriptionCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(" | ", validation.Errors);
                return Result<Guid>.Failure($"Erreurs de validation : {errors}");
            }
            var classe = await _classeRepository.GetClasseByIdAsync(request.IdClasse, cancellationToken);
            if (classe is null)
                return Result<Guid>.Failure("Classe introuvable.");

            var inscription = classe.TrouverInscriptionParEleve(request.IdEleve);
            if (inscription is null)
                return Result<Guid>.Failure("Inscription introuvable.");

            var idInscription = inscription.Id;
            classe.SupprimerInscription(request.IdEleve);
            await _inscRepo.DeleteAsync(idInscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(idInscription);
        }
    }

}
