using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.UpdateEleve
{
    public record UpdateEleveDto(
        Guid EleveId,
        string Rue,
        string CodePostal,
        string Ville,
        string Tel1,
        string? Tel2,
        string EmailParent
        );
}
