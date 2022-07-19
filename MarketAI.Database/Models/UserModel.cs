using MarketAI.API.Enums;
using MarketAI.API.Models.Stats;
using MarketAI.API.Models.WB;
using MarketAI.Database.Models;
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


        public string ReferralCode { get; set; } = Guid.NewGuid().ToString().Replace("-","");
        public decimal ReferralBalance { get; set; }
        public List<ReferralModel> Referrals { get; set; } = new List<ReferralModel>();

        public string Surname { get; set; }
        public string AvatarPath { get; set; } = "/img/avatar.jpg";
        public string Activity { get; set; }

        public string Name { get; set; }



        public bool IsOnline { get; set; }

        public UserData UserData { get; set; }
        public int? UserDataId { get; set; }


        public List<WBAPITokenModel> WBAPIKeys { get; set; } = new List<WBAPITokenModel>();
        public List<TicketModel> Tickets { get; set; } = new List<TicketModel>();
        public List<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
        public List<WithdrawRequest> WithdrawRequests { get; set; } = new List<WithdrawRequest>();
        public List<AuthStatsModel> Auths { get; set; } = new List<AuthStatsModel>();

        public List<ExtraExpenseModel> ExtraExpenses { get; set; } = new List<ExtraExpenseModel>();
        public List<SelfCostModel> SelfCosts { get; set; } = new List<SelfCostModel>();


        public bool WasTrialActivated { get; set; }
        public DateTime SubscriptionBefore { get; set; }
        [NotMapped]
        public bool IsSubscriptionEnded => SubscriptionBefore < DateTime.UtcNow;

        public bool WasPromocodeActivated { get; set; }
        public string ActivatedPromocode { get; set; }


        public string GetRefferalLink()
        {
            return $"https://localhost:44363/Registration?invite={ReferralCode}";
        }
        public int GetPaidRefferalsCount()
        {
            int count = 0;
            foreach (var referral in Referrals)
            {
                if(referral.HisRef.Payments.Count > 0)
                    count++;
            }
            return count;
        }

    }
}
