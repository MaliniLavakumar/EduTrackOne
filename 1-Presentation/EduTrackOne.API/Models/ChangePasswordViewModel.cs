using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.API.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        public string Identifiant { get; set; } = default!;
        [Required, DataType(DataType.Password)]
        [Display(Name = "Ancien mot de passe")]
        public string AncienMotDePasse { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NouveauMotDePasse { get; set; }

    }
}
