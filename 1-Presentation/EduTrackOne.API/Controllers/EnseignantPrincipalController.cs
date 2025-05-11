using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EduTrackOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnseignantPrincipalController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EnseignantPrincipalController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnseignantPrincipalDto dto)
        {
            var cmd = new CreateEnseignantPrincipalCommand(
                dto.Prenom, dto.Nom, dto.Email);

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(Create),
                new { id = result.Value },
                new { Id = result.Value });
        }
    }
}
