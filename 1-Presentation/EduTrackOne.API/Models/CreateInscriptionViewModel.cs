using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.API.Models
{
    public class CreateInscriptionViewModel
    {
        [Required]
        public Guid ClasseId { get; set; }

        // Nom de la classe récupéré pour l’affichage
        public string ClasseName { get; set; } = "";

        [Required(ErrorMessage = "Le numéro d'immatricule est requis.")]
        [StringLength(50, ErrorMessage = "Le numéro d'immatricule ne peut dépasser 50 caractères.")]
        public string NoImmatricule { get; set; } = "";

        [Required(ErrorMessage = "La date de début est requise.")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }
    }
}
