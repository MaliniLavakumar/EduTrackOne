namespace EduTrackOne.API.Models
{
    public class ClasseDetailsViewModel
    {

        public Guid ClassId { get; set; }
        public string NomClasse { get; set; }
        public IEnumerable<EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse.GetInscriptionsByClasseDto> Inscriptions { get; set; }
    }
}
