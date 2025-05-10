using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Matieres;
using EduTrackOne.Domain.Matieres.Events;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace EduTrackOne.Application.Matieres.CreateMatiere
{
    public class CreateMatiereCommandHandler : IRequestHandler<CreateMatiereCommand, Result<Guid>> 
    {
        private readonly IMatiereRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateMatiereCommand> _validator;
        

        public CreateMatiereCommandHandler(IMatiereRepository repository, IUnitOfWork uow, IValidator<CreateMatiereCommand> validator)
        {
            _repository = repository;
            _unitOfWork = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(CreateMatiereCommand command, CancellationToken cancellationToken)
        { // 1)validation
            var v = await _validator.ValidateAsync(command, cancellationToken);
            if (!v.IsValid)
            {
                var errs = string.Join(" | ", v.Errors);
                return Result<Guid>.Failure($"Validation échouée : {errs}");
            }

            // 2) unicité
            if (await _repository.ExistsAsync(command.Nom, cancellationToken))
                return Result<Guid>.Failure("Une matière avec ce nom existe déjà.");

            // 3) création domaine
            var id = Guid.NewGuid();
            var matiere = Matiere.Creer(id, new NomMatiere(command.Nom));

            // 4) persistance
            await _repository.AddAsync(matiere, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5) retour
            return Result<Guid>.Success(id);
        }
    }
}
