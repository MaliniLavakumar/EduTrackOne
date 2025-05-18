using EduTrackOne.Domain.Utilisateurs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid utilisateurId, string identifiant, IEnumerable<string>roles);
    }

}
