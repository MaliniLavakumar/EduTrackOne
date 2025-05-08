using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Inscriptions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse
{
    public class GetInscriptionsByClasseQueryHandler : IRequestHandler<GetInscriptionsByClasseQuery, Result<List<GetInscriptionsByClasseDto>>>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly IValidator<GetInscriptionsByClasseQuery> _validator;

        public GetInscriptionsByClasseQueryHandler(
            IInscriptionRepository inscRepo,
            IValidator<GetInscriptionsByClasseQuery> validator)
        {
            _inscRepo = inscRepo;
            _validator = validator;
        }

        public async Task<Result<List<GetInscriptionsByClasseDto>>> Handle(
            GetInscriptionsByClasseQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
                return Result<List<GetInscriptionsByClasseDto>>.Failure($"Validation échouée : {errors}");
            }

            var inscriptions = await _inscRepo.GetByClasseAsync(request.ClasseId, cancellationToken);
            var dtos = inscriptions.Select(i => new GetInscriptionsByClasseDto(
                i.Id,
                i.IdEleve,
                i.Eleve.NomComplet.ToString()                
            )).ToList();

            return Result<List<GetInscriptionsByClasseDto>>.Success(dtos);
        }

    }
}
