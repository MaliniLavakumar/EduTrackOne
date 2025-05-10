using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Matieres
{
    public interface IMatiereRepository
    {
        Task AddAsync(Matiere matiere, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string nom, CancellationToken cancellationToken = default);
        Task<List<Matiere>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
