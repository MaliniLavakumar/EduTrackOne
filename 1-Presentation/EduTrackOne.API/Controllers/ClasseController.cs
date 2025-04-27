using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.DeleteClasse;
using EduTrackOne.Application.Classes.Queries.GetClasseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClasseById(Guid id)
        {
            var result = await _mediator.Send(new GetClasseByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteClasseCommand(id));

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

    }
}
