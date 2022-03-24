using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Admin
{
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;
        public TicketsController(ILogger<TicketsController> logger)
        {
            _logger = logger;
        }

        [Route("Admin/Tickets")]
        public IActionResult Tickets()
        {
            return View("Views/Admin/Tickets/Tickets.cshtml");
        }
        [Route("Admin/CreateMessage")]
        public IActionResult CreateMessage()
        {
            return View("Views/Admin/Tickets/CreateMessage.cshtml");
        }
    }
}
