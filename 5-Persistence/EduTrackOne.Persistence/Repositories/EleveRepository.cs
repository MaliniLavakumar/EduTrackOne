using EduTrackOne.Domain.Eleves;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class EleveRepository : IEleveRepository
    {
        private readonly EduTrackOneDbContext _context;
        public EleveRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }
        public async Task<Eleve?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Eleves
                    .Include(e => e.Inscriptions)
                    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task AddAsync(Eleve eleve)
        {
            await _context.Eleves.AddAsync(eleve);
        }

        public Task UpdateCoordonneesAsync(Eleve eleve)
        {
            _context.Eleves.Update(eleve);
            return Task.CompletedTask;
        }
        public async Task<bool> ExistsByNoImmatriculeAsync(string noImmatricule, CancellationToken cancellationToken = default)
        {
            return await _context.Eleves
                                 .AsNoTracking()
                                 .AnyAsync(e => e.NoImmatricule == noImmatricule, cancellationToken);
        }
    }
}
