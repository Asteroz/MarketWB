using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAI.API.Models.WB
{
    public class AvailableWBCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string APIKey { get; set; }
        public string Category { get; set; }
    }
}
