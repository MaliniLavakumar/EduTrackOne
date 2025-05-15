using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.AddNotesForClasse
{
    public class AddNotesForClasseCommandHandler : IRequestHandler<AddNotesForClasseCommand, Result<int>>
    {
        private readonly IInscriptionRepository _inscRepo;
        private readonly INoteRepository _noteRepo;
        private readonly IInscriptionManager _manager;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<AddNotesForClasseCommand> _validator;

        public AddNotesForClasseCommandHandler(
            IInscriptionRepository inscRepo,
            INoteRepository noteRepo,
            IInscriptionManager manager,
            IUnitOfWork uow,
            IValidator<AddNotesForClasseCommand> validator)
        {
            _inscRepo = inscRepo;
            _noteRepo = noteRepo;
            _manager = manager;
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(
            AddNotesForClasseCommand cmd,
            CancellationToken ct)
        {
            var v = await _validator.ValidateAsync(cmd, ct);
            if (!v.IsValid)
                return Result<int>.Failure(string.Join(" | ", v.Errors));

            // 1. Charger toutes les inscriptions de la classe
            var inscriptions = await _inscRepo.GetByClasseAsync(cmd.ClasseId, ct);
            if (!inscriptions.Any())
                return Result<int>.Failure("Aucune inscription trouvée pour la classe.");

            Guid? lastNoteId = null;
            int count = 0;
            foreach (var noteDto in cmd.Notes)
            {
                var insc = inscriptions.FirstOrDefault(i => i.IdEleve == noteDto.EleveId);
                if (insc == null) continue;

                var noteId = Guid.NewGuid();
                var valeur = noteDto.Valeur.HasValue
                    ? new ValeurNote(noteDto.Valeur.Value)
                    : ValeurNote.Absent();
                var commentaire = noteDto.Commentaire is null
                    ? null
                    : new CommentaireEvaluation(noteDto.Commentaire);

                var note = new Note(
                    noteId,
                    cmd.DateExamen,
                    valeur,
                    commentaire,
                    insc.Id,
                    noteDto.MatiereId
                );

                _manager.AjouterNote(insc, note);
                await _noteRepo.AddAsync(note, ct);                
                count++;
            }

            await _uow.SaveChangesAsync(ct);
            return Result<int>.Success(count);
        }
    }
}
