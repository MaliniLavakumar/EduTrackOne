using EduTrackOne.Domain.Presences;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class PresenceRepository: IPresenceRepository
    {
        private readonly EduTrackOneDbContext _context;

        public PresenceRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Presence presence, CancellationToken cancellationToken = default)
        {
            if (presence == null) throw new ArgumentNullException(nameof(presence));
            await _context.Presences.AddAsync(presence, cancellationToken);
        }

        public async Task<List<Presence>> GetByInscriptionIdAsync(Guid inscriptionId, CancellationToken cancellationToken = default)
        {
            return await _context.Presences
                .Where(p => p.IdInscription == inscriptionId)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var p = await _context.Presences.FindAsync(new object[] { id }, cancellationToken);
            if (p != null) _context.Presences.Remove(p);
        }
    }
}
