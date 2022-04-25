using MarketAI.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class PromocodeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public double Percent { get; set; }

        public PromocodeType Type { get; set; }

        public PromocodeModel Clone()
        {
            return new PromocodeModel
            {
                Id = Id,
                Code = Code,
                Percent = Percent
            };
        }
    }
}
