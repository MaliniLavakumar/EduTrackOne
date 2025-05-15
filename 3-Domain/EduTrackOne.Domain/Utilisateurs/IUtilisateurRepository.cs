using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs
{
    public interface IUtilisateurRepository
    {
        Task AddAsync(Utilisateur user, CancellationToken cancellationToken = default);
        void Delete(Utilisateur user);
        Task<Utilisateur?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Utilisateur?> GetByIdentifiantAsync(string identifiant, CancellationToken cancellationToken = default);
        Task<List<Utilisateur>> GetAllAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
