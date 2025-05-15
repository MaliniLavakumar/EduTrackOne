using EduTrackOne.Application.Inscriptions.UpdateNote;
using EduTrackOne.Application.Inscriptions.UpdatePresence;
using EduTrackOne.Application.Notes.GetNotesByInscription;
using EduTrackOne.Application.Presences;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Presences;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers
{
    [ApiController]
    [Route("api/inscriptions/{inscriptionId:guid}")]
    public class InscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InscriptionController(IMediator mediator) => _mediator = mediator;

        //--- NOTES

        [HttpPut("notes/{noteId:guid}")]
        //public async Task<IActionResult> UpdateNote(
        //    Guid inscriptionId,
        //    Guid noteId,
        //    [FromBody] UpdateNoteDto dto)
        //{
        //    if (inscriptionId != dto.InscriptionId || noteId != dto.NoteId)
        //        return BadRequest("IDs mismatch");

        //    var cmd = new UpdateNoteCommand(
        //        dto.InscriptionId,
        //        dto.NoteId,
        //        dto.DateExamen,
        //        dto.MatiereId,
        //        dto.Valeur,
        //        dto.Commentaire
        //    );
        //    var result = await _mediator.Send(cmd);
        //    if (!result.IsSuccess) return BadRequest(result.Error);
        //    return Ok();
        //}

        //--- PRÉSENCES

        [HttpPut("presences/{presenceId:guid}")]
        public async Task<IActionResult> UpdatePresence(
            Guid inscriptionId,
            Guid presenceId,
            [FromBody] UpdatePresenceDto dto,
            CancellationToken ct)
        {
            if (inscriptionId != dto.InscriptionId || presenceId != dto.PresenceId)
                return BadRequest("IDs mismatch");
            //var statut = StatutPresence.FromString(dto.Statut);
            var cmd = new UpdatePresenceCommand(
                InscriptionId: dto.InscriptionId,
                PresenceId: dto.PresenceId,
                Date: dto.Date,
                Periode: dto.Periode,
                Statut: dto.Statut
            );
            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok();
        }

        [HttpGet("presences")]
        public async Task<IActionResult> GetPresences(Guid inscriptionId)
        {
            var query = new GetPresencesByInscriptionQuery(inscriptionId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET api/inscriptions/{inscriptionId}/notes
        [HttpGet("notes")]
        public async Task<IActionResult> GetNotes(
            Guid inscriptionId,
            [FromQuery] Guid? matiereId,
            [FromQuery] DateTime? dateDebut,
            [FromQuery] DateTime? dateFin,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetNotesByInscriptionQuery(
                inscriptionId,
                matiereId,
                dateDebut,
                dateFin,
                pageNumber,
                pageSize
            );

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}
