using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class TicketMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UserModel SentBy { get; set; }
        public int SentById { get; set; }
        public TicketModel Owner { get; set; }
        public int OwnerId { get; set; }
        public string Message { get; set; }
        public string AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } 
    }
}
