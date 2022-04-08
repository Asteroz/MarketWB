using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        public TicketController(ILogger<TicketController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IEnumerable<TicketModel> GetAllTickets()
        {
            using (APIDBContext db = new APIDBContext())
            {
                
                return db.Tickets.Include(o => o.OpenedBy).Include(o => o.Messages).ToList();
            }
        }
        [HttpGet]
        public IEnumerable<TicketModel> GetUserTickets(int userId)
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.Tickets.Include(o => o.OpenedBy).Where(o => o.OpenedBy.Id == userId).ToList();
            }
        }
        [HttpPost]
        public async Task<RequestStatus> OpenTicket(int userId,TicketModel ticket)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                    if (foundUser is null)
                        return new RequestStatus("Пользователя с таким id не существует", 404);

                    ticket.CreatedAt = DateTime.Now;
                    foundUser.Tickets.Add(ticket);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }

        [HttpPost]
        public async Task<RequestStatus> AddTicketMessage(TicketModel ticket,TicketMessage msg)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    msg.OwnerId = ticket.Id;
                    msg.CreatedAt = DateTime.Now;

                    ticket.Messages.Add(msg);
                    db.Tickets.Update(ticket);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> CloseTicket(int ticketId)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var fountTicket = db.Tickets.FirstOrDefault(o => o.Id == ticketId);
                    if (fountTicket is null)
                        return new RequestStatus("Тикет с таким id не существует", 404);

                    fountTicket.IsClosed = true;
                    db.Tickets.Update(fountTicket);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPost]
        public async Task<RequestStatus> CreateTicketMessage(int ticketId,int fromUserId,TicketMessage msg)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == fromUserId);
                    if (foundUser is null)
                        return new RequestStatus("Пользователя с таким id не существует", 404);

                    var foundTicket = db.Tickets.FirstOrDefault(o => o.Id == ticketId);
                    if (foundTicket is null)
                        return new RequestStatus("Тикет с таким id не существует", 404);

                    msg.Owner = foundTicket;
                    msg.SentBy = foundUser;

                    foundTicket.Messages.Add(msg);
                    db.Tickets.Update(foundTicket);

                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
    }
}
