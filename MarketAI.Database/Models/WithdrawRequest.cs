using MarketAI.API.Models;
using MarketAI.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAI.Database.Models
{
    public class WithdrawRequest
    {
        [Key]
        public int Id { get; set; }

        public UserModel User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Sum { get; set; }
        public string AccountNumber { get; set; }
        public WithdrawStatus Status { get; set; } = WithdrawStatus.Waiting;
    }
}
