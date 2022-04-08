using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAI.API.Models
{
    public class UserData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public WBAPITokenModel SelectedWBAPIToken { get; set; }
        public int SelectedWBAPITokenId { get; set; }

        public UserModel Owner { get; set; }
        public int OwnerId { get; set; }


        public DateTime SelectedPeriodFrom { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime SelectedPeriodTo { get; set; } = DateTime.Now;
    }
}
