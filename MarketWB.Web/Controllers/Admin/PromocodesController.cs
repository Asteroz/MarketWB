using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Admin
{
    public class PromocodesController : Controller
    {
        private readonly ILogger<PromocodesController> _logger;
        public PromocodesController(ILogger<PromocodesController> logger)
        {
            _logger = logger;
        }
        [Route("Admin/Promocodes")]
        public IActionResult Promocodes()
        {
            return View("Views/Admin/Promocodes/Promocodes.cshtml");
        }
        [Route("Admin/CreatePromocode")]
        public IActionResult CreatePromocode()
        {
            return View("Views/Admin/Promocodes/CreatePromocode.cshtml");
        }
        [Route("Admin/UpdatePromocode")]
        public IActionResult UpdatePromocode()
        {
            return View("Views/Admin/Promocodes/UpdatePromocode.cshtml");
        }
    }
}
