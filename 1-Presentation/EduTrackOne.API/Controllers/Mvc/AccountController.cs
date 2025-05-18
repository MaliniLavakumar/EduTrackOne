using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackOne.API.Controllers.Mvc
{
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // si cookie auth
            // si tu stockes le token dans un cookie, tu le supprimes ici
            return RedirectToAction("Login", "Utilisateur");
        }
    }
}
