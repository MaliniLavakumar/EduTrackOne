using EduTrackOne.Application.Presences;

namespace EduTrackOne.API.Models
{
    public class PresenceReportViewModel
    {
        public string SchoolName { get; set; } = string.Empty;
        public string ClasseName { get; set; } = string.Empty;
        public string EleveName { get; set; } = string.Empty;
        public string NoImmatricule { get; set; } = string.Empty;

        public int PeriodesPresentes { get; set; } 
        public IReadOnlyList<AbsenceDto> Absences { get; set; } = new List<AbsenceDto>();
    }
}
