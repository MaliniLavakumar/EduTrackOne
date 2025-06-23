using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Eleves.GetEleveByImmatricule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    [Authorize]
    [Route("Inscription")]
    public class InscriptionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InscriptionController> _logger;
        public InscriptionController(IMediator mediator, ILogger<InscriptionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: /Inscription/Create?classeId={id}
        [HttpGet("Create")]
        public IActionResult Create(Guid classeId)
        {
            _logger.LogInformation("GET Inscription/Create - début pour ClasseId={ClasseId}", classeId);

            var cmd = new AddInscriptionCommand(
                ClasseId: classeId,
                NoImmatricule: string.Empty,
                DateDebut: DateTime.Today,
                DateFin: null
            );
            _logger.LogInformation("GET Inscription/Create - fin préparation AddInscriptionCommand pour ClasseId={ClasseId}", classeId);
            return View(cmd);
        }

        // POST: /Inscription/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddInscriptionCommand cmd)
        {
            _logger.LogInformation("POST Inscription/Create - début pour ClasseId={ClasseId}, Immatricule={Immatricule}, DateDebut={DateDebut}, DateFin={DateFin}",
                cmd.ClasseId, cmd.NoImmatricule, cmd.DateDebut, cmd.DateFin);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST Inscription/Create - ModelState invalide pour ClasseId={ClasseId}", cmd.ClasseId);
                return View(cmd);
            }


            try
            {
                var result = await _mediator.Send(cmd);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("AddInscriptionCommand échoué pour ClasseId={ClasseId}, Immatricule={Immatricule}: {Error}",
                        cmd.ClasseId, cmd.NoImmatricule, result.Error);
                    ModelState.AddModelError(string.Empty, result.Error);
                    return View(cmd);
                }

                _logger.LogInformation("Inscription ajoutée avec succès pour ClasseId={ClasseId}, EleveImmatricule={Immatricule}",
                    cmd.ClasseId, cmd.NoImmatricule);
                TempData["SuccessMessage"] = "Inscription ajoutée avec succès !";
                _logger.LogInformation("Redirection vers Classe/Details après inscription, ClasseId={ClasseId}", cmd.ClasseId);
                return RedirectToAction("Details", "Classe", new { id = cmd.ClasseId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception inattendue dans POST Inscription/Create pour ClasseId={ClasseId}, Immatricule={Immatricule}",
                    cmd.ClasseId, cmd.NoImmatricule);
                ModelState.AddModelError(string.Empty, "Erreur interne lors de l’ajout de l’inscription.");
                return View(cmd);
            }
        }


    }

}
