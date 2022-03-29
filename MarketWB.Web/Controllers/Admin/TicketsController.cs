using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tickets = MarketAI.API.Controllers.TicketController;

namespace MarketWB.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly Tickets _api;
        public TicketsController(ILogger<TicketsController> logger, Tickets api)
        {
            _logger = logger;
            _api = api;
        }

        [Route("Admin/Tickets")]
        public IActionResult Tickets()
        {
            var tickets = _api.GetAllTickets();
            return View("Views/Admin/Tickets/Tickets.cshtml", tickets);
        }



        [Route("Admin/CreateMessage")]
        public IActionResult CreateMessage()
        {
            return View("Views/Admin/Tickets/CreateMessage.cshtml");
        }
    }
}
