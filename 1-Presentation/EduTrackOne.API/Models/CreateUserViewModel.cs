using EduTrackOne.Contracts.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.API.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Le login est obligatoire.")]
        [Display(Name = "Login")]
        public string Identifiant { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public UserRoleDto? Role { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public UserStatusDto? Statut { get; set; }

    }
}

