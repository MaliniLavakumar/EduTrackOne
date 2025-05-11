using EduTrackOne.Domain.Notes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly EduTrackOneDbContext _context;

        public NoteRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Note note, CancellationToken cancellationToken)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            await _context.Notes.AddAsync(note, cancellationToken);
                      
        }

        /// <summary>
        /// Récupère une note par son identifiant.
        /// </summary>
        public async Task<Note?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        }

        /// <summary>
        /// Récupère toutes les notes liées à une inscription donnée.
        /// </summary>
        public async Task<List<Note>> GetByInscriptionIdAsync(
            Guid inscriptionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Notes
                .Where(n => n.IdInscription == inscriptionId)
                .ToListAsync(cancellationToken);
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var note = await _context.Notes.FindAsync(new object[] { id }, cancellationToken);
            if (note != null)
                _context.Notes.Remove(note);
        }

       public Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
        {
            _context.Notes.Update(note);
            return Task.CompletedTask;
        }
    }

}
