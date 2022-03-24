using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class SubscriptionModel
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int Days { get; set; }
    }
}
