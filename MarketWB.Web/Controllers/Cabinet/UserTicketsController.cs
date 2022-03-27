using MarketAI.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tickets = MarketAI.API.Controllers.TicketController;

namespace MarketWB.Web.Controllers.Cabinet
{
    public class UserTicketsController : Controller
    {
        private readonly ILogger<UserTicketsController> _logger;
        private readonly Tickets _api;
        public UserTicketsController(ILogger<UserTicketsController> logger, Tickets api)
        {
            _logger = logger;
            _api = api;
        }

        [Route("Cabinet/MyTickets")]
        public IActionResult MyTickets()
        {
            var tickets = _api.GetAllTickets().Where(o => true);
            return View("Views/Cabinet/Tickets/MyTickets.cshtml", tickets);
        }
        [Route("Cabinet/CreateTicket")]
        public IActionResult ShowTicket(TicketModel ticket)
        {
            return View("Views/Cabinet/Tickets/TicketPage.cshtml", ticket);
        }

        [Route("Cabinet/CreateTicket")]
        public IActionResult CreateTicket()
        {
            return View("Views/Cabinet/Tickets/CreateTicket.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicketPost(TicketModel ticket)
        {
            //TODO: АЙДИ !!!
            await _api.OpenTicket(0,ticket);
            return ShowTicket(ticket);
        }
    }
}
