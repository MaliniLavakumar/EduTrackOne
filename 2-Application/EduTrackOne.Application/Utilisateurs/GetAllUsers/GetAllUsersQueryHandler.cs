using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Utilisateurs.GetAllUsers
{
    public class GetAllUsersQueryHandler:IRequestHandler<GetAllUsersQuery, IEnumerable<GetAllUsersDto>>
    {
        private readonly IUtilisateurRepository _repo;
        public GetAllUsersQueryHandler(IUtilisateurRepository repo) => _repo = repo;

        public async Task<IEnumerable<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
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
