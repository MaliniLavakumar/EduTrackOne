using EduTrackOne.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace EduTrackOne.Application.Classes.CreateClasse
{
    public record CreateClasseCommand(
         string NomClasse,
         string AnneeScolaire,
         Guid? IdEnseignantPrincipal
     ) :IRequest<Result<Guid>>;
}
