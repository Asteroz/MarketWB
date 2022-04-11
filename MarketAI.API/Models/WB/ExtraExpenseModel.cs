using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAI.API.Models.WB
{
    public class ExtraExpenseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public WBAPITokenModel WBAPIToken { get; set; }
        public int WBAPITokenId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string ClearingCentre { get; set; }
        public string Category { get; set; }
        public DateTime Period { get; set; } = DateTime.UtcNow;
        public double Sum { get; set; }
        public string PaymentReceiver { get; set; }
        public string PaymentDescription { get; set; }
        public string Comment { get; set; }
    }
}
