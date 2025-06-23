using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.API.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        public string Identifiant { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Ancien mot de passe")]
        public string AncienMotDePasse { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} caractères.", MinimumLength = 6)]
        public string NouveauMotDePasse { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmation du mot de passe")]
        [Compare("NouveauMotDePasse", ErrorMessage = "Le nouveau mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmationMotDePasse { get; set; } = "";
    }
}
