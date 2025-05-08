using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes;
using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.DeleteClasse;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;

namespace EduTrackOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClasseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClasseController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClasseDto dto)
        {
            // 1. Validation automatique par FluentValidation
            // 2. Construction de la commande
            var cmd = new CreateClasseCommand(
                dto.NomClasse,
                dto.AnneeScolaire,
                dto.IdEnseignantPrincipal
            );

            // 3. Envoi de la commande au handler
            var result = await _mediator.Send(cmd);

            // 4. Gestion du résultat
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            // 5. Retourne 201 Created avec la route pour récupérer la ressource
            return CreatedAtAction(
                nameof(GetClasseById),
                new { id = result.Value },
                new { Id = result.Value }
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteClasseCommand(id));

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
        /// <summary>
        /// Ajoute une inscription (élève → classe).
        /// </summary>
        /// <param name="classeId">Identifiant de la classe (dans l'URL).</param>
        /// <param name="dto">Données d'inscription (dont ClasseId doit matcher l'URL).</param>
        [HttpPost("{classeId:guid}/inscriptions")]
        public async Task<IActionResult> AddInscription(
            [FromRoute] Guid classeId,
            [FromBody] AddInscriptionDto dto,
            CancellationToken cancellationToken)
        {
            // Vérifier la cohérence URL vs DTO
            if (classeId != dto.ClasseId)
                return BadRequest("L'ID de la classe dans l'URL ne correspond pas à celui du corps.");

            // Construire et envoyer la commande
            var cmd = new AddInscriptionCommand(dto);
            Result<Guid> result = await _mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            // Renvoie 201 Created pointant sur la classe modifiée
            return CreatedAtAction(
                nameof(GetClasseById),
                new { id = classeId },
                new { InscriptionId = result.Value }
            );
        }

        /// <summary>
        /// Récupère une classe avec ses inscriptions.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetClasseById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetClasseByIdQuery(id);
            Result<ClasseDto> result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpDelete("{classeId:guid}/inscriptions/{eleveId:guid}")]
        public async Task<IActionResult> RemoveInscription(
            [FromRoute] Guid classeId,
            [FromRoute] Guid eleveId,
            CancellationToken cancellationToken)
        {
            // Construction et envoi de la commande
            var cmd = new RemoveInscriptionCommand(classeId, eleveId);
            Result<Guid> result = await _mediator.Send(cmd, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Ok(new { InscriptionId = result.Value });
        }
        [HttpGet("{classeId:guid}/inscriptions")]
        public async Task<IActionResult> GetInscriptions(
        Guid classeId,
        CancellationToken cancellationToken)
        {
            var query = new GetInscriptionsByClasseQuery(classeId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }
    }
}
