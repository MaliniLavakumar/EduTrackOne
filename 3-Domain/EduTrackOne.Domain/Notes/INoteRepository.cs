using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Notes
{
    public interface INoteRepository
    {
        Task AddAsync(Note note, CancellationToken ct = default);
        Task<Note> GetByIdAsync(Guid Id, CancellationToken ct= default);
        Task<List<Note>> GetByInscriptionIdAsync(Guid inscriptionId, CancellationToken ct = default);
        Task DeleteAsync(Guid Id, CancellationToken ct = default);
        Task UpdateAsync(Note note, CancellationToken ct = default);

    }
}
