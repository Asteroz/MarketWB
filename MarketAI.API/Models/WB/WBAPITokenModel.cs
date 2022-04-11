using MarketAI.API.Models.WB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class WBAPITokenModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool CreatedFirstTime { get; set; }

        public UserModel Owner { get; set; }
        public string Name { get; set; }
        public string APIKey { get; set; }
        public List<SelfCostModel> SelfCosts { get; set; } = new List<SelfCostModel>();
        public List<ExtraExpenseModel> ExtraExpenses { get; set; } = new List<ExtraExpenseModel>();
    }
}
