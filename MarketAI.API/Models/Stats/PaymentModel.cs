using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models.Stats
{
    public class PaymentModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public UserModel User { get; set; }
        public double Sum { get; set; }
        public string Description { get; set; }
 
    }
}
