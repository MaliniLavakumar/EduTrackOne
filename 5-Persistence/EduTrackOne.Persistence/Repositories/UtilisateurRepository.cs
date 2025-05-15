using EduTrackOne.Domain.Utilisateurs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class UtilisateurRepository: IUtilisateurRepository
    {
        private readonly EduTrackOneDbContext _context;

        public UtilisateurRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Utilisateur user, CancellationToken cancellationToken = default)
        {
            await _context.Utilisateurs.AddAsync(user, cancellationToken);
        }

        public void Delete(Utilisateur user)
        {
            _context.Utilisateurs.Remove(user);
        }

        public async Task<Utilisateur?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<Utilisateur?> GetByIdentifiantAsync(string identifiant, CancellationToken cancellationToken = default)
        {
            return await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Identifiant == identifiant, cancellationToken);
        }

        public async Task<List<Utilisateur>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Utilisateurs.ToListAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
