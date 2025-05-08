using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Eleves
{
    public interface IEleveRepository
    {
        Task<Eleve?> GetByIdAsync(Guid id, CancellationToken cancellationToken =default);
        Task AddAsync(Eleve eleve, CancellationToken ct = default);
        Task UpdateCoordonneesAsync(Eleve eleve, CancellationToken ct = default);
        Task<bool> ExistsByNoImmatriculeAsync(string noImmatricule, CancellationToken cancellationToken = default);
        Task<Eleve?> GetByNoImmatriculeAsync(string noImmatricule, CancellationToken cancellationToken =default);
    }
}
