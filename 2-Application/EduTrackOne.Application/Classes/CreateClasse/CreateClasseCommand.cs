using EduTrackOne.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace EduTrackOne.Application.Classes.Commands.CreateClasse
{
    public class CreateClasseCommand : IRequest<Result<Guid>>
    {
        public string NomClasse { get; set; }
        public string AnneeScolaire { get; set; }
    }
}
