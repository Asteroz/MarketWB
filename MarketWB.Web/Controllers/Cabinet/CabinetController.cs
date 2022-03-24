using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers
{
    public class CabinetController : Controller
    {
        private readonly ILogger<CabinetController> _logger;
        public CabinetController(ILogger<CabinetController> logger)
        {
            _logger = logger;
        }

        [Route("Cabinet/Profile")]
        public IActionResult Profile()
        {
            return View("Views/Cabinet/Profile.cshtml");
        }
        [Route("Cabinet/About")]
        public IActionResult About()
        {
            return View("Views/Cabinet/About.cshtml");
        }
        [Route("Cabinet/Support")]
        public IActionResult Support()
        {
            return View("Views/Cabinet/Support.cshtml");
        }
        [Route("Cabinet/Analysis")]
        public IActionResult Analysis()
        {
            return View("Views/Cabinet/Analysis.cshtml");
        }
    }
}
