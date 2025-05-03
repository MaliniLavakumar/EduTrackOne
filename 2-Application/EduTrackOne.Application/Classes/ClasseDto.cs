using System;

namespace EduTrackOne.Application.Classes
{
    public record ClasseDto
    (
        Guid Id,
        string NomClasse,
        string AnneeScolaire,
        Guid? IdEnseignantPrincipal
    );

}
