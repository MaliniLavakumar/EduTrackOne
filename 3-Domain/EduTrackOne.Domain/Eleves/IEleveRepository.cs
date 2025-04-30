using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Eleves
{
    public interface IEleveRepository
    {
        Task<Eleve?> GetByIdAsync(Guid id);
        Task AddAsync(Eleve eleve);
        Task UpdateCoordonneesAsync(Eleve eleve);
        Task<bool> ExistsByNoImmatriculeAsync(string noImmatricule, CancellationToken cancellationToken = default);
    }
}
