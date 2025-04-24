using EduTrackOne.Application.Classes.Commands.CreateClasse;
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
        public async Task<IActionResult> CreateClasse([FromBody] CreateClasseCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetClasseById), new { id = result.Value }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClasseById(Guid id)
        {
            var result = await _mediator.Send(new GetClasseByIdQuery(id));

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

    }
}
