using EduTrackOne.Domain.Matieres;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class MatiereRepository : IMatiereRepository
    {
        private readonly EduTrackOneDbContext _context;

        public MatiereRepository(EduTrackOneDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Matiere matiere, CancellationToken cancellationToken)
        {
            await _context.Matieres.AddAsync(matiere, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string nom, CancellationToken cancellationToken)
        {
            return await _context.Matieres
                .AnyAsync(m => m.Nom.Value == nom.Trim(), cancellationToken);
        }
        public async Task<List<Matiere>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Matieres.ToListAsync(cancellationToken);
        }


    }
}
