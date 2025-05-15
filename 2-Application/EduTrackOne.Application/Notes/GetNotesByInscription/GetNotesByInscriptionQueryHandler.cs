using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Notes.GetNotesByInscription
{
    public class GetNotesByInscriptionQueryHandler : IRequestHandler<GetNotesByInscriptionQuery, PaginatedResult<GetNotesByInscriptionDto>>
    {
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly IMatiereRepository _matiereRepo;

        public GetNotesByInscriptionQueryHandler(
            IInscriptionRepository inscriptionRepo,
            IMatiereRepository matiererRepo)
        {
            _inscriptionRepo = inscriptionRepo;
            _matiereRepo = matiererRepo;
        }
        public async Task<PaginatedResult<GetNotesByInscriptionDto>> Handle(
    GetNotesByInscriptionQuery q,
    CancellationToken ct)
        {
            var insc = await _inscriptionRepo.GetByIdAsync(q.InscriptionId, ct)
                        ?? throw new KeyNotFoundException("Inscription introuvable.");

            var matieres = await _matiereRepo.GetAllAsync(ct);
            var moyennes = insc.CalculerMoyennesParMatiere();

            // on filtre d’abord les notes
            var notes = insc.Notes.AsQueryable();

            if (q.MatiereId.HasValue)
                notes = notes.Where(n => n.IdMatiere == q.MatiereId.Value);
            if (q.DateDebut.HasValue)
                notes = notes.Where(n => n.DateExamen.Date >= q.DateDebut.Value.Date);
            if (q.DateFin.HasValue)
                notes = notes.Where(n => n.DateExamen.Date <= q.DateFin.Value.Date);

            // on groupe, projette, et pagine
            var grouped = notes
                .GroupBy(n => n.IdMatiere)
                .Select(g => new GetNotesByInscriptionDto(
                    g.Key,
                    matieres.First(m => m.Id == g.Key).Nom.Value,
                    g.Select(n => new NoteDto(
                    n.Id,
                    n.DateExamen,
                    n.Valeur.Value, 
                    n.Valeur.EstAbsent
                    )).OrderBy(n => n.DateExamen),
                    moyennes.GetValueOrDefault(g.Key)
                    ));


            // Pagination (Skip/Take)
            var totalItems = grouped.Count();
            var pageItems = grouped
                .Skip((q.PageNumber - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToList();

            return new PaginatedResult<GetNotesByInscriptionDto>(
                PageNumber: q.PageNumber,
                PageSize: q.PageSize,
                TotalItems: totalItems,
                Items: pageItems
            );
        }

    }
}
