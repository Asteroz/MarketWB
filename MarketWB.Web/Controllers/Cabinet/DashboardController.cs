using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Cabinet
{
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }
        [Route("Cabinet/Dashboard/Analysis")]
        public IActionResult Analysis()
        {
            return View("Views/Cabinet/Dashboard/Analysis.cshtml");
        }
        [Route("Cabinet/Dashboard/Products")]
        public IActionResult Products()
        {
            return View("Views/Cabinet/Dashboard/Products.cshtml");
        }
        [Route("Cabinet/Dashboard")]
        public IActionResult Dashboard()
        {
            return View("Views/Cabinet/Dashboard/Dashboard.cshtml");
        }
        [Route("Cabinet/Dashboard/Orders")]
        public IActionResult Orders()
        {
            return View("Views/Cabinet/Dashboard/Orders.cshtml");
        }
        [Route("Cabinet/Dashboard/Sales")]
        public IActionResult Sales()
        {
            return View("Views/Cabinet/Dashboard/Sales.cshtml");
        }
        [Route("Cabinet/Dashboard/CheckList")]
        public IActionResult CheckList()
        {
            return View("Views/Cabinet/Dashboard/CheckList.cshtml");
        }


        [Route("Cabinet/Dashboard/SelfCosts")]
        public IActionResult SelfCosts()
        {
            return View("Views/Cabinet/Dashboard/SelfCosts.cshtml");
        }
    }
}
