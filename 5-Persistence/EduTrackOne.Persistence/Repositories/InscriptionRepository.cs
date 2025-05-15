using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Persistence.Repositories
{
    public class InscriptionRepository : IInscriptionRepository
    {
        private readonly EduTrackOneDbContext _context;
        private readonly IEleveRepository _eleveRepo;
        private readonly IClasseRepository _classeRepo;

        public InscriptionRepository(EduTrackOneDbContext context, IEleveRepository eleveRepo, IClasseRepository classeRepo)
        {
            _context = context;
            _eleveRepo = eleveRepo;
            _classeRepo = classeRepo;
        }

        public async Task AddAsync(Inscription inscription, CancellationToken cancellationToken = default)
        {
            await _context.Inscriptions.AddAsync(inscription, cancellationToken);
        }

        public async Task<Inscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var insc = await _context.Inscriptions
            .Include(i => i.Notes)       // nécessite un mapping HasMany
            .Include(i => i.Presences)  // de même
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (insc is not null)
            {
                var eleve = await _eleveRepo.GetByIdAsync(insc.IdEleve, cancellationToken);
                var classe = await _classeRepo.GetClasseByIdAsync(insc.IdClasse, cancellationToken);

                insc.SetEleve(eleve!);
                insc.SetClasse(classe!);
            }

            return insc;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var insc = await _context.Inscriptions.FindAsync(new object[] { id }, cancellationToken);
            if (insc != null)
            {
                _context.Inscriptions.Remove(insc);
            }
        }
        public async Task<List<Inscription>> GetByClasseAsync(Guid classeId, CancellationToken cancellationToken = default)
        {
            var inscr = await _context.Inscriptions
                .Where(i => i.IdClasse == classeId)
                .ToListAsync(cancellationToken);
            foreach (var i in inscr)
            {
                var eleve = await _eleveRepo.GetByIdAsync(i.IdEleve, cancellationToken);
                i.SetEleve(eleve!);
            }
            return inscr;
        }
        public Task UpdateAsync(Inscription insc, CancellationToken ct = default)
        {
            _context.Inscriptions.Update(insc);
            return Task.CompletedTask;
        }
    }
}