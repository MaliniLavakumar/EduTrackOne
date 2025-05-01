using EduTrackOne.Application.Common;
using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.GetEleveById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EduTrackOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EleveController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EleveController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEleveDto dto)
        {
            var cmd = new CreateEleveCommand(
                 dto.Prenom,
                 dto.Nom,
                 dto.DateNaissance,
                 dto.Sexe,
                 dto.Rue,
                 dto.CodePostal,
                 dto.Ville,                
                 dto.EmailParent,
                 dto.Tel1,
                 dto.Tel2,
                 dto.NoImmatricule
             );

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(Create),
                new { id = result.Value },
                new { Id = result.Value });
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEleveByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}

