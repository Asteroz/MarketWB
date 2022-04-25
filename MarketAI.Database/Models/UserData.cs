using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAI.API.Models
{
    public class UserData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public List<WBAPITokenModel> SelectedWBAPITokens { get; set; } = new List<WBAPITokenModel>();

        public DateTime SelectedPeriodFrom { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime SelectedPeriodTo { get; set; } = DateTime.Now;

        public string? SelectedWBBrand { get; set; }
        public string? SelectedWBCategory { get; set; }
    }
}
