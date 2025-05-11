using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.AddInscription
{
   public record AddInscriptionCommand(Guid ClasseId,
        string NoImmatricule,
        DateTime DateDebut,
        DateTime? DateFin) : IRequest<Result<Guid>>;
    
}
