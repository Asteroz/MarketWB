using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class WBAPITokenModel
    {
        [Key]
        public int Id { get; set; }
        public UserModel Owner { get; set; }
        public string Name { get; set; }
        public string APIKey { get; set; }
    }
}
