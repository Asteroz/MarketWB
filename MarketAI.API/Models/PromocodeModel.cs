using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class PromocodeModel
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public double Percent { get; set; }
    }
}
