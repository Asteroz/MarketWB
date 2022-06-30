using MarketAI.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAI.Database.Models
{
    public class SubscriptionDescriptionItem
    {
        [Key]
        public int Id { get; set; }
        public SubscriptionSign Sign { get; set; } = SubscriptionSign.CheckMark;
        public string Text { get; set; }
    }
}
