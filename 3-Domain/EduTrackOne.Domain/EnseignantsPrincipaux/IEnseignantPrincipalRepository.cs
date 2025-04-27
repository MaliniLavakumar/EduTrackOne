using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.EnseignantsPrincipaux
{
    public interface IEnseignantPrincipalRepository
    {

        Task AddAsync(EnseignantPrincipal enseignant);
        Task<EnseignantPrincipal?> GetByIdAsync(Guid id);
        Task<List<EnseignantPrincipal>> GetAllAsync();
        Task SaveChangesAsync();

    }
}
