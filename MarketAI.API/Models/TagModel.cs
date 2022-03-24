using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class TagModel
    {
        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
    }
}
