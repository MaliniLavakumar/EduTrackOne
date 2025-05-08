using EduTrackOne.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class ClasseRepository : IClasseRepository
    {
        private readonly EduTrackOneDbContext _context;

        public ClasseRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddClasseAsync(Classe classe, CancellationToken ct = default)
        {
            await _context.Classes.AddAsync(classe);
        }

        public async Task DeleteClasseAsync(Guid id, CancellationToken ct = default)
        {
            var classe = await _context.Classes.FindAsync(id);
            if (classe != null)
            {
                _context.Classes.Remove(classe);
            }
        }

        public async Task<Classe?> GetClasseByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .Include(c => c.Inscriptions)
                .Include(c => c.EnseignantPrincipal)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Classe>> GetClasseParEnseignantAsync(Guid idEnseignant)
        {
            return await _context.Classes
                .Where(c => c.IdEnseignantPrincipal == idEnseignant)
                .ToListAsync();
        }

        public async Task<Classe?> GetByAnneeAsync(AnneeScolaire anneeScolaire)
        {
            return await _context.Classes
                .FirstOrDefaultAsync(c => c.AnneeScolaire.Value == anneeScolaire.Value);
        }

        public Task UpdateClasseAsync(Classe classe, CancellationToken cancellationToken = default)
        {
            _context.Classes.Update(classe);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync();
        }
    }
}
