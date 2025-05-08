
using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Classes.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.DeleteClasse
{
    public class DeleteClasseCommandHandler : IRequestHandler<DeleteClasseCommand, Result<Guid>>
    {
        private readonly IClasseRepository _classeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteClasseCommandHandler(IClasseRepository classeRepository, IUnitOfWork unitOfWork)
        {
            _classeRepository = classeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteClasseCommand request, CancellationToken cancellationToken)
        {
            var classe = await _classeRepository.GetClasseByIdAsync(request.ClasseId);
            if (classe == null)
                return Result<Guid>.Failure("Classe introuvable.");

            classe.SupprimerClasse();

            await _classeRepository.DeleteClasseAsync(classe.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(classe.Id);
        }
    }
}
