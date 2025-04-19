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
        Task<Classe?>GetClasseByIdAsync(Guid id);
        Task<IReadOnlyList<Classe>> GetClasseParEnseignantAsync(Guid idEnseignant);
        Task AddClasseAsync(Classe classe);
        Task UpdateClasseAsync(Classe classe);
        Task DeleteClasseAsync(Guid id);

    }
}
