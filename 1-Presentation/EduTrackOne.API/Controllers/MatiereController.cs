using EduTrackOne.Application.Matieres.CreateMatiere;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using EduTrackOne.Application.Common;
using EduTrackOne.Application.Matieres.GetAllMatieres;

namespace EduTrackOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatiereController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatiereController(IMediator mediator)
            => _mediator = mediator;

        // POST api/matieres
        [HttpPost]
        public async Task<IActionResult> CreateMatiere([FromBody] CreateMatiereDto dto)
        {
            var cmd = new CreateMatiereCommand(dto.NomMatiere);
            var result = await _mediator.Send(cmd);

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            // 201 Created → on pointe vers GetMatiereById
            return CreatedAtAction(
                nameof(GetMatiereById),
                new { id = result.Value },
                new { id = result.Value }
            );
        }

        // GET api/matieres/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMatiereById(Guid id)
        {
            // à implémenter un GetMatiereByIdQuery
            // pour l'instant stub
            return Ok(new { Id = id, Nom = "Exemple" });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMatieres()
        {
            var result = await _mediator.Send(new GetAllMatieresQuery());
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }
    }
}

