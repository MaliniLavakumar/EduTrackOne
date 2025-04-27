using System;

namespace EduTrackOne.Application.Classes
{
    public class ClasseDto
    {
        public Guid Id { get; set; }
        public string NomClasse { get; set; } = default!;
        public string AnneeScolaire { get; set; } = default!;
        public Guid? IdEnseignantPrincipal { get; set; }
    }

}
