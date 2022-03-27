using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]
    public class StatsController : Controller
    {
        private readonly ILogger<StatsController> _logger;
        public StatsController(ILogger<StatsController> logger)
        {
            _logger = logger;
        }

        [Route("Admin/Visitors")]
        public IActionResult Visitors()
        {
            return View("Views/Admin/Stats/Visitors.cshtml");
        }
        [Route("Admin/Payments")]
        public IActionResult Payments()
        {
            return View("Views/Admin/Stats/Payments.cshtml");
        }
        [Route("Admin/Online")]
        public IActionResult Online()
        {
            return View("Views/Admin/Stats/Online.cshtml");
        }
    }
}
