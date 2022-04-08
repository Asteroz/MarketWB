using MarketAI.API.Models;

namespace MarketAI.API.ViewModels
{
    public class TicketPageVM
    {
        public TicketModel Ticket { get; set; }
        public TicketMessage NewMessage { get; set; } = new TicketMessage();
    }
}
