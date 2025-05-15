using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Application.Utilisateurs.CreateUser;
using EduTrackOne.Application.Utilisateurs.DeleteUser;
using EduTrackOne.Application.Utilisateurs.GetAllUsers;
using EduTrackOne.Application.Utilisateurs.LoginUser;
using EduTrackOne.Application.Utilisateurs.UpdateUser;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UtilisateurController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // POST api/user/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _mediator.Send(new LoginUserCommand(dto));
            return Ok(new { token });
        }

        // POST api/user
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var userId = await _mediator.Send(new CreateUserCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = userId }, null);
        }

        // GET api/user/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var dtos = await _mediator.Send(new GetAllUsersQuery());
            return Ok(dtos);
        }

        // PUT api/utilisateur/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(
            Guid id,
            [FromBody] UpdateUserDto dto)
        {
            if (id != dto.Id)
                return BadRequest("L'identifiant de l'URL doit correspondre à celui du payload.");

            await _mediator.Send(new UpdateUserCommand(dto));
            return NoContent();
        }
        // POST api/utilisateur/change-password
        [HttpPost("change-password")]
        [Authorize] // Admin ou lui‑même, le handler le contrôle
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto)
        {
            await _mediator.Send(new ChangePasswordCommand(dto));
            return NoContent();
        }

        // DELETE api/utilisateur/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return NoContent();
        }
    }
}
