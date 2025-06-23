using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Classes;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetClassesByEnseignantPrincipalQueryHandler> _logger;

        public GetClassesByEnseignantPrincipalQueryHandler(
            IClasseRepository classeRepo,
            ILogger<GetClassesByEnseignantPrincipalQueryHandler> logger)
        {
            _classeRepo = classeRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<ClassInfoDto>> Handle(
            GetClassesByEnseignantPrincipalQuery request,
            CancellationToken ct)
        {
            _logger.LogInformation("Recherche des classes pour l'enseignant principal avec l'ID utilisateur : {UserId}", request.PrincipalUserId);

            var classes = await _classeRepo
                .GetClasseParEnseignantAsync(request.PrincipalUserId);

            _logger.LogInformation("{Count} classe(s) trouvée(s) pour l'enseignant avec l'ID utilisateur : {UserId}", classes.Count(), request.PrincipalUserId);

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
