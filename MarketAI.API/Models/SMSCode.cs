using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class SMSCode
    {
        [Key]
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
    }
}
