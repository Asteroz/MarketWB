using MarketAI.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAI.Database.Models
{
    public class ReferralModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public UserModel Owner { get; set; }
        public int OwnerId { get; set; }

        public UserModel HisRef { get; set; }
        public int HisRefId { get; set; }
    }
}
