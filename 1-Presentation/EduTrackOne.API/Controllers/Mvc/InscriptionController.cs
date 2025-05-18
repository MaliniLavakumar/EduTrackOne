using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Eleves.GetEleveByImmatricule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Route("Inscription")]
    public class InscriptionController : Controller
    {
        private readonly IMediator _mediator;
        public InscriptionController(IMediator mediator) => _mediator = mediator;

        // GET: /Inscription/Create?classeId={id}
        [HttpGet("Create")]
        public IActionResult Create(Guid classeId)
        {
            
            var cmd = new AddInscriptionCommand(
                ClasseId: classeId,
                NoImmatricule: string.Empty,
                DateDebut: DateTime.Today,
                DateFin: null
            );
            return View(cmd);
        }

        // POST: /Inscription/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddInscriptionCommand cmd)
        {
            if (!ModelState.IsValid)
                return View(cmd);
            

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return View(cmd);
            }

            TempData["SuccessMessage"] = "Inscription ajoutée avec succès !";
        
            return RedirectToAction("Details", "Classe", new { id = cmd.ClasseId });
        }
              

    }

}
