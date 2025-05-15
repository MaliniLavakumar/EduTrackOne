using EduTrackOne.Application.Common;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.AddPresencesForClasse
{
    public record AddPresencesForClasseCommand(
    Guid ClasseId,
    DateTime Date,
    int Periode,
    List<PresenceEleveDto> Presences
    
) : IRequest<Result<int>>;

}
