using Microsoft.AspNetCore.Mvc;

namespace MarketWB.Web.Controllers
{
    public class ErrorsController : Controller
    {
        [Route("Error/NotFound")]
        public IActionResult NotFound()
        {
            return View("Views/Errors/404.cshtml");
        }
    }
}
