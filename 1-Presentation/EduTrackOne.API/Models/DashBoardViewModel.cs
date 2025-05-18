namespace EduTrackOne.API.Models
{

    public record DashboardViewModel(
            bool IsAdmin,
            string SchoolName,
            IEnumerable<ClassInfoDto>? ClassesToManage
            );
    public record ClassInfoDto(
            Guid Id,
            string NomClasse
            );
}


