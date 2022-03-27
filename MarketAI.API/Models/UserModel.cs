using MarketAI.API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public UserRole UserRole { get; set; } = UserRole.User;
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }


        public List<WBAPITokenModel> WBAPIKeys { get; set; } = new List<WBAPITokenModel>();
        public List<TicketModel> Tickets { get; set; } = new List<TicketModel>();
        public DateTime SubscriptionBefore { get; set; }
        public bool WasPromocodeActivated { get; set; }
        public string ActivatedPromocode { get; set; }
    }
}
