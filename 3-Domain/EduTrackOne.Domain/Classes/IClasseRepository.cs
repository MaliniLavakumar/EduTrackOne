using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Classes
{
    public interface IClasseRepository
    {
        Task<Classe?>GetClasseByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Classe>> GetClasseParEnseignantAsync(Guid idEnseignant);
        Task AddClasseAsync(Classe classe, CancellationToken cancellationToken = default);
        Task UpdateClasseAsync(Classe classe, CancellationToken cancellationToken=default);
        Task DeleteClasseAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Classe?> GetByAnneeAsync(AnneeScolaire anneeScolaire);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
