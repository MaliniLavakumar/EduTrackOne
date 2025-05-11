using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Presences
{
    public interface IPresenceRepository
    {
        Task AddAsync(Presence presence, CancellationToken cancellationToken = default);
        Task<List<Presence>> GetByInscriptionIdAsync(Guid inscriptionId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
