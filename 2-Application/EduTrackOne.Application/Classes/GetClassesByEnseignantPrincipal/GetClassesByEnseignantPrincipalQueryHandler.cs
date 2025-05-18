using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Classes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal
{
    public class GetClassesByEnseignantPrincipalQueryHandler
     : IRequestHandler<GetClassesByEnseignantPrincipalQuery, IEnumerable<ClassInfoDto>>
    {
        private readonly IClasseRepository _classeRepo;

        public GetClassesByEnseignantPrincipalQueryHandler(IClasseRepository classeRepo)
        {
            _classeRepo = classeRepo;
        }

        public async Task<IEnumerable<ClassInfoDto>> Handle(
            GetClassesByEnseignantPrincipalQuery request,
            CancellationToken ct)
        {        
            var classes = await _classeRepo
                .GetClasseParEnseignantAsync(request.PrincipalUserId);

            // 2) Mappe vers DTO
            return classes
                .Select(c => new ClassInfoDto(
                    c.Id,
                    c.Nom.Value               
                ))
                .ToList();
        }
    }

}
