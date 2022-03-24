using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class TicketMessage
    {
        [Key]
        public int Id { get; set; }
        public UserModel SentBy { get; set; }
        public TicketModel Owner { get; set; }
        public string Message { get; set; }
    }
}
