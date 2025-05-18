using EduTrackOne.Domain.EnseignantsPrincipaux;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class EnseignantPrincipalRepository : IEnseignantPrincipalRepository
    {
        private readonly EduTrackOneDbContext _context;

        public EnseignantPrincipalRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EnseignantPrincipal enseignant)
        {
            await _context.EnseignantsPrincipaux.AddAsync(enseignant);
        }

        public async Task<EnseignantPrincipal?> GetByIdAsync(Guid id)
        {
            return await _context.EnseignantsPrincipaux.FindAsync(id);
        }

        public async Task<List<EnseignantPrincipal>> GetAllAsync()
        {
            return await _context.EnseignantsPrincipaux.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<EnseignantPrincipal>> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.EnseignantsPrincipaux
                             .Include(ep => ep.Classes)    // suppose relation navigationnelle
                             .Where(ep => ep.Email.Value == email)
                             .ToListAsync(ct);
        }
    }
}
