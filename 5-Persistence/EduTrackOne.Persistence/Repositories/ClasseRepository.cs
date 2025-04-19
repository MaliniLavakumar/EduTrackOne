using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduTrackOne.Domain.Classes;

namespace EduTrack.Persistence.Repositories
{
    public class ClasseRepository : IClasseRepository
    {
        private readonly EduTrackDbContext _context;

        public ClasseRepository(EduTrackDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Implémentation de GetClasseByIdAsync
        public async Task<Classe?> GetClasseByIdAsync(Guid id)
        {
            return await _context.Classes
                                 .Where(c => c.Id == id)
                                 .FirstOrDefaultAsync();
        }

        // Implémentation de GetClasseParEnseignantAsync
        public async Task<IReadOnlyList<Classe>> GetClasseParEnseignantAsync(Guid idEnseignant)
        {
            return await _context.Classes
                                 .Where(c => c.EnseignantId == idEnseignant)
                                 .ToListAsync();
        }

        // Implémentation de AddClasseAsync
        public async Task AddClasseAsync(Classe classe)
        {
            await _context.Classes.AddAsync(classe);
            await _context.SaveChangesAsync();
        }

        // Implémentation de UpdateClasseAsync
        public async Task UpdateClasseAsync(Classe classe)
        {
            _context.Classes.Update(classe);
            await _context.SaveChangesAsync();
        }

        // Implémentation de DeleteClasseAsync
        public async Task DeleteClasseAsync(Guid id)
        {
            var classe = await _context.Classes.FindAsync(id);
            if (classe != null)
            {
                _context.Classes.Remove(classe);
            }
        }
    }
}
