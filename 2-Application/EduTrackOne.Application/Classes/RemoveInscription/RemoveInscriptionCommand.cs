using MediatR;
using EduTrackOne.Application.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Classes.RemoveInscription
{
    public record RemoveInscriptionCommand(Guid IdClasse, Guid IdEleve) : IRequest<Result<Guid>>;
    
}
