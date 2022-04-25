using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class TicketModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UserModel OpenedBy { get; set; }
        public int OpenedById { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsClosed { get; set; }
        public List<TicketMessage> Messages { get; set; } = new List<TicketMessage>();

        public DateTime CreatedAt { get; set; }
    }


}
