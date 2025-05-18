using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Contracts.DTOs
{
    namespace EduTrackOne.Contracts.DTOs
    {
        public record LoginUserResponseDto(string Token, string Identifiant, IList<string> Roles);
    }

}
