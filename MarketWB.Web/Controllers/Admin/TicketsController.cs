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

namespace MarketWB.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly Tickets _api;
        public TicketsController(ILogger<TicketsController> logger, Tickets api, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _api = api;
            _appEnvironment = appEnvironment;
        }

        [Route("Admin/Tickets")]
        public IActionResult Tickets()
        {
            var tickets = _api.GetAllTickets();
            return View("Views/Admin/Tickets/Tickets.cshtml", tickets);
        }



        [Route("Admin/CreateMessage")]
        public IActionResult CreateMessage(int ticketId)
        {
            var ticket = _api.GetAllTickets().FirstOrDefault(o => o.Id == ticketId);
            var vm = new TicketPageVM
            {
                Ticket = ticket,
            };
            return View("Views/Admin/Tickets/CreateMessage.cshtml", vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddTicketMessage(int ticketid, TicketMessage msg)
        {
            var ticket = _api.GetAllTickets().FirstOrDefault(o => o.Id == ticketid);

            await SetAttachmentIfHas(msg);

            msg.Message = Request.Form["Message"];
            msg.SentBy = await UserHelper.GetUser(User);
            msg.SentById = msg.SentBy.Id;


           await _api.AddTicketMessage(ticket, msg);
            return CreateMessage(ticket.Id);
        }
        [HttpGet]
        public async Task<IActionResult> CloseTicket(int ticketid)
        {
            await _api.CloseTicket(ticketid);
            return Tickets();
        }
        [HttpGet]
        public async Task<IActionResult> GetAttachment(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + path);
            string fileName = Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
