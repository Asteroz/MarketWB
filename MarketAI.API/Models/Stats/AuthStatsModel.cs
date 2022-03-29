using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models.Stats
{
    public class AuthStatsModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public UserModel User { get; set; }
    }
}
