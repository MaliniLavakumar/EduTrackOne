using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Inscriptions.Events;
using EduTrackOne.Domain.Notes;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.UpdateNote
{
    public class UpdateNoteCommandHandler
    : IRequestHandler<UpdateNoteCommand, Result<Guid>>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly IUnitOfWork _uow;
        private readonly IInscriptionManager _manager;
        private readonly IValidator<UpdateNoteCommand> _validator;

        public UpdateNoteCommandHandler(
            IInscriptionRepository inscRepo,
            IUnitOfWork uow,
            IInscriptionManager manager,
            IValidator<UpdateNoteCommand> validator)
        {
            _inscRepo = inscRepo;
            _uow = uow;
            _manager = manager;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(UpdateNoteCommand cmd, CancellationToken ct)
        {
            var v = await _validator.ValidateAsync(cmd, ct);
            if (!v.IsValid)
                return Result<Guid>.Failure(string.Join(" | ", v.Errors));

            var insc = await _inscRepo.GetByIdAsync(cmd.InscriptionId, ct);
            if (insc == null)
                return Result<Guid>.Failure("Inscription introuvable.");

            var nouvelleValeur = cmd.NouvelleValeur.HasValue
                ? new ValeurNote(cmd.NouvelleValeur.Value)
                : ValeurNote.Absent();
            var commentaire = string.IsNullOrWhiteSpace(cmd.NouveauCommentaire)
                 ? null
                 : new CommentaireEvaluation(cmd.NouveauCommentaire);


            // Ici on passe par le manager
            _manager.ModifierNote(
                insc,
                cmd.NoteId,
                cmd.NouvelleDateExamen,
                cmd.NouvelleMatiereId,
                nouvelleValeur,
                commentaire
            );

          
            await _uow.SaveChangesAsync(ct);
            return Result<Guid>.Success(cmd.NoteId);
        }
    }

}
