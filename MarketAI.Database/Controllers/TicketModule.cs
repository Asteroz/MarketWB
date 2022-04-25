using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class TicketModule
    {
        private readonly ILogger<TicketModule> _logger;
        private readonly APIDBContext db;
        public TicketModule(ILogger<TicketModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }


        public IEnumerable<TicketModel> GetAllTickets()
        {
            return db.Tickets.Include(o => o.OpenedBy).Include(o => o.Messages).ToList();
        }
        public IEnumerable<TicketModel> GetUserTickets(int userId)
        {
            return db.Tickets.Include(o => o.OpenedBy).Where(o => o.OpenedBy.Id == userId).ToList();
        }
        public async Task<RequestStatus> OpenTicket(int userId,TicketModel ticket)
        {
            try
            {
                var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                if (foundUser is null)
                    return new RequestStatus("Пользователя с таким id не существует", 404);

                ticket.CreatedAt = DateTime.Now;
                foundUser.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }

        public async Task<RequestStatus> AddTicketMessage(TicketModel ticket,TicketMessage msg)
        {
            try
            {
                msg.Owner = ticket;
                msg.OwnerId = ticket.Id;
                msg.CreatedAt = DateTime.Now;

                ticket.Messages.Add(msg);

                db.Tickets.Update(ticket);
                await db.SaveChangesAsync();
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }

        public async Task<RequestStatus> CloseTicket(int ticketId)
        {
            try
            {
                var fountTicket = db.Tickets.FirstOrDefault(o => o.Id == ticketId);
                if (fountTicket is null)
                    return new RequestStatus("Тикет с таким id не существует", 404);

                fountTicket.IsClosed = true;
                db.Tickets.Update(fountTicket);
                await db.SaveChangesAsync();
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }

        public async Task<RequestStatus> CreateTicketMessage(int ticketId,int fromUserId,TicketMessage msg)
        {
            try
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
                return new RequestStatus("Тикет успешно создан");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
    }
}
