using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;

namespace EduTrackOne.API.Models
{
    public class ClasseDetailsViewModel
    {

        public Guid ClassId { get; set; }
        public string NomClasse { get; set; }
        public IEnumerable<GetInscriptionsByClasseDto> Inscriptions { get; set; }
    }
}
