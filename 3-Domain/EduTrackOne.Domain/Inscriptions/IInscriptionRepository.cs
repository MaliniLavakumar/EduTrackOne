using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions
{
    public interface IInscriptionRepository
    {
        Task AddAsync(Inscription inscription, CancellationToken cancellationToken = default);
        Task<Inscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Inscription>> GetByClasseAsync(Guid classeId, CancellationToken cancellationToken = default);
    }
}
