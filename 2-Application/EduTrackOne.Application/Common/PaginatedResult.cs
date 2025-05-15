using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Common
{
    public record PaginatedResult<T>(
     int PageNumber,
     int PageSize,
     int TotalItems,
     IReadOnlyList<T> Items
 );

}
