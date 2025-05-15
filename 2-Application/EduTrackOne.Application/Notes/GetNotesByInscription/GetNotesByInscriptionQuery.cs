using EduTrackOne.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Notes.GetNotesByInscription
{
    public class GetNotesByInscriptionQuery : IRequest<PaginatedResult<GetNotesByInscriptionDto>>
    {
        public Guid InscriptionId { get; }
        public Guid? MatiereId { get; }              // filtre optionnel
        public DateTime? DateDebut { get; }
        public DateTime? DateFin { get; }
        public int PageNumber { get; } = 1;          // pagination
        public int PageSize { get; } = 10;

        public GetNotesByInscriptionQuery(
            Guid inscriptionId,
            Guid? matiereId = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            InscriptionId = inscriptionId;
            MatiereId = matiereId;
            DateDebut = dateDebut;
            DateFin = dateFin;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

}
