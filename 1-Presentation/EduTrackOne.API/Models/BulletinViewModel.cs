namespace EduTrackOne.API.Models
{
    public class BulletinViewModel
    {
        public string SchoolName { get; set; }
        public string EleveName { get; set; }
        public string NoImmatricule { get; set; }
        public string ClasseName { get; set; }
        public List<MatiereBulletinDto> Matieres { get; set; }
    }

    public class MatiereBulletinDto
    {
        public string MatiereNom { get; set; }
        public double Moyenne { get; set; }
    }
}