using MarketAI.API.Enums;
using MarketAI.API.Models.Stats;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UserRole UserRole { get; set; } = UserRole.User;
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }


        public bool IsOnline { get; set; }

        public UserData UserData { get; set; }
        public int? UserDataId { get; set; }


        public List<WBAPITokenModel> WBAPIKeys { get; set; } = new List<WBAPITokenModel>();
        public List<TicketModel> Tickets { get; set; } = new List<TicketModel>();
        public List<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
        public List<AuthStatsModel> Auths { get; set; } = new List<AuthStatsModel>();

        public DateTime SubscriptionBefore { get; set; }
        [NotMapped]
        public bool IsSubscriptionEnded => SubscriptionBefore < DateTime.UtcNow;

        public bool WasPromocodeActivated { get; set; }
        public string ActivatedPromocode { get; set; }
    }
}
