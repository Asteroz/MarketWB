using MarketAI.API.Models;
using MarketAI.API.ViewModels;
using MarketWB.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tickets = MarketAI.API.Controllers.TicketController;

namespace MarketWB.Web.Controllers.Cabinet
{
    [Authorize(Roles = "User")]
    public class UserTicketsController : Controller
    {
        private readonly ILogger<UserTicketsController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly Tickets _api;
        public UserTicketsController(ILogger<UserTicketsController> logger, Tickets api, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _api = api;
            _appEnvironment = appEnvironment;
        }

        [Route("Cabinet/MyTickets")]
        public IActionResult MyTickets()
        {
            var tickets = _api.GetAllTickets().Where(o => o.OpenedBy != null && o.OpenedBy.Id == UserHelper.GetUser(User).Id);
            return View("Views/Cabinet/Tickets/MyTickets.cshtml", tickets);
        }
        [Route("Cabinet/ShowTicket")]
        public IActionResult ShowTicket(int id)
        {
            var ticket = _api.GetAllTickets().FirstOrDefault(o => o.Id == id);
            var vm = new TicketPageVM
            {
                Ticket = ticket,
            };
            return View("Views/Cabinet/Tickets/TicketPage.cshtml", vm);
        }

        [Route("Cabinet/CreateTicket")]
        public IActionResult CreateTicket()
        {
            return View("Views/Cabinet/Tickets/CreateTicket.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicketPost(TicketModel ticket)
        {
            await SetAttachmentIfHas(ticket);
            ticket.OpenedBy = UserHelper.GetUser(User);

            await _api.OpenTicket(ticket.OpenedBy.Id, ticket);
            return ShowTicket(ticket.Id);
        }
        [HttpPost]
        public async Task<IActionResult> AddTicketMessage(int ticketid,TicketMessage msg)
        {
            var ticket = _api.GetAllTickets().FirstOrDefault(o => o.Id == ticketid);

            await SetAttachmentIfHas(msg);

            msg.Message = Request.Form["Message"];
            msg.SentById = ticket.OpenedById;

            await _api.AddTicketMessage(ticket, msg);
            return ShowTicket(ticket.Id);
        }



        [HttpGet]
        public async Task<IActionResult> GetAttachment(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath+path);
            string fileName = Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        private async Task SetAttachmentIfHas(TicketModel ticket)
        {
            var attachment = Request.Form.Files.FirstOrDefault();
            var filePath = "/uploads/" + Guid.NewGuid().ToString();

            if (attachment != null)
            {
                ticket.AttachmentPath = filePath + attachment.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + ticket.AttachmentPath, FileMode.Create))
                {
                    await attachment.CopyToAsync(fileStream);
                }
            }
        }
        private async Task SetAttachmentIfHas(TicketMessage ticket)
        {
            var attachment = Request.Form.Files.FirstOrDefault();
            var filePath = "/uploads/" + Guid.NewGuid().ToString();

            if (attachment != null)
            {
                ticket.AttachmentPath = filePath + attachment.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + ticket.AttachmentPath, FileMode.Create))
                {
                    await attachment.CopyToAsync(fileStream);
                }
            }

        }
    }
}
