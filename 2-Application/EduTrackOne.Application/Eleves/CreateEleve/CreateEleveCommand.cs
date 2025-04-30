using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.CreateEleve
{
    public record CreateEleveCommand(
        string Prenom,
        string Nom,
        DateTime DateNaissance,
        string Sexe,
        string Rue,
        string CodePostal,
        string Ville,       
        string EmailParent,
        string Tel1,
        string? Tel2,
        string NoImmatricule
    ) : IRequest<Result<Guid>>;

}
