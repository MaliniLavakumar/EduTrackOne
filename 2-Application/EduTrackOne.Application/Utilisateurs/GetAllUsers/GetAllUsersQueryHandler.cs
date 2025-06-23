using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<GetAllUsersDto>>
    {
        private readonly IUtilisateurRepository _repo;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(IUtilisateurRepository repo, ILogger<GetAllUsersQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            _logger.LogInformation("GetAllUsersQuery démarrée");

            var users = await _repo.GetAllAsync(ct);
           
            return users.Select(u => new GetAllUsersDto(
                u.Id,
                u.Identifiant,
                u.Role.Valeur,
                u.Statut.Value,
                u.Email.Value));
        }
    }
}
