using EduTrackOne.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTrackOne.API.Models
{
    public class NotesForClasseViewModel
    {
        public Guid ClasseId { get; set; }
        public DateTime DateExamen { get; set; }       
        public Guid SelectedMatiereId { get; set; }
        public List<SelectListItem> Matieres { get; set; } = new();       
        public List<EleveNoteEntry> Notes { get; set; } = new();
    }
    public class EleveNoteEntry
    {
        public Guid EleveId { get; set; }
        public string EleveName { get; set; } = "";       
        public double? Valeur { get; set; }
        public string? Commentaire { get; set; }       
        public Guid MatiereId { get; set; }
    }
}
