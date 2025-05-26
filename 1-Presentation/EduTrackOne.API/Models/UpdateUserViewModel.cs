using EduTrackOne.Contracts.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.API.Models
{
    public class UpdateUserViewModel
    {
        [Required] public Guid Id { get; set; }
        [Required] public string Identifiant { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public UserRoleDto Role { get; set; }
        [Required] public UserStatusDto Statut { get; set; }
    }

}
