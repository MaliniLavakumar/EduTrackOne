using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AddNotesForClasseCommandHandler> _logger;
        public AddNotesForClasseCommandHandler(
            IInscriptionRepository inscRepo,
            INoteRepository noteRepo,
            IInscriptionManager manager,
            IUnitOfWork uow,
            IValidator<AddNotesForClasseCommand> validator,
            ILogger<AddNotesForClasseCommandHandler> logger)
        {
            _inscRepo = inscRepo;
            _noteRepo = noteRepo;
            _manager = manager;
            _uow = uow;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(
            AddNotesForClasseCommand cmd,
            CancellationToken ct)
        {
            _logger.LogInformation("Début de l'ajout des notes pour la classe {ClasseId} à la date {DateExamen}.", cmd.ClasseId, cmd.DateExamen);

            var v = await _validator.ValidateAsync(cmd, ct);
            if (!v.IsValid)
            {
                var errors = string.Join(" | ", v.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation échouée pour AddNotesForClasseCommand : {Errors}", errors);
                return Result<int>.Failure(errors);
            }
            // 1. Charger toutes les inscriptions de la classe
            var inscriptions = await _inscRepo.GetByClasseAsync(cmd.ClasseId, ct);
            if (!inscriptions.Any())
            {
                _logger.LogWarning("Aucune inscription trouvée pour la classe {ClasseId}.", cmd.ClasseId);
                return Result<int>.Failure("Aucune inscription trouvée pour la classe.");
            }

            Guid? lastNoteId = null;
            int count = 0;
            foreach (var noteDto in cmd.Notes)
            {
                var insc = inscriptions.FirstOrDefault(i => i.IdEleve == noteDto.EleveId);
                if (insc == null)
                {
                    _logger.LogWarning("Élève {EleveId} non trouvé dans les inscriptions de la classe {ClasseId}.", noteDto.EleveId, cmd.ClasseId);
                    continue;
                }

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
                _logger.LogInformation("Note ajoutée pour élève {EleveId}, noteId : {NoteId}.", noteDto.EleveId, noteId);
                count++;
            }

            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("{Count} note(s) ajoutée(s) avec succès pour la classe {ClasseId}.", count, cmd.ClasseId);
            return Result<int>.Success(count);
        }
    }
}
