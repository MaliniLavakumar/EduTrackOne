namespace EduTrackOne.API.Models
{
    public class PresencesForClasseViewModel
    {
        public Guid ClasseId { get; set; }
        public DateTime Date { get; set; }
        public int Periode { get; set; }

        // Entrées par élève
        public List<ElevePresenceEntry> Presences { get; set; } = new();
    }

    public class ElevePresenceEntry
    {
        public Guid EleveId { get; set; }
        public string EleveName { get; set; } = "";

        // Statut à choisir (Present / Absent)
        public string Statut { get; set; } = "";
    }
}

