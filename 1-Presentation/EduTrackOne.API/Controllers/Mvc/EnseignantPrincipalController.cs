using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("EnseignantsPrincipaux")]
    public class EnseignantPrincipalController : Controller
    {
        private readonly IMediator _mediator;
        public EnseignantPrincipalController(IMediator mediator)
        {
            _mediator = mediator;
        }
        

        // GET: /EnseignantsPrincipaux/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /EnseignantsPrincipaux/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEnseignantPrincipalDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var cmd = new CreateEnseignantPrincipalCommand(
                dto.Prenom,
                dto.Nom,
                dto.Email);

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(dto);
            }
            TempData["SuccessMessage"] = "Enseignant principal créé avec succès !";
            return RedirectToAction("Index", "Home");

        }
    }
}
