using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WildberriesAPI.Models
{
    public class WBOrderModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string APIKey { get; set; }
        public bool IsSelfBuy { get; set; }

        [JsonProperty("number")]
        public long? Number { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("lastChangeDate")]
        public DateTime LastChangeDate { get; set; }
        [JsonProperty("supplierArticle")]
        public string SupplierArticle { get; set; }
        [JsonProperty("techSize")]
        public string TechSize { get; set; }
        [JsonProperty("barcode")]
        public string Barcode { get; set; }
        [JsonProperty("quantity")]
        public long? Quantity { get; set; }
        [JsonProperty("totalPrice")]
        public double? TotalPrice { get; set; }
        [JsonProperty("discountPercent")]
        public double? DiscountPercent { get; set; }
        [JsonProperty("warehouseName")]
        public string WarehouseName { get; set; }
        [JsonProperty("oblast")]
        public string Oblast { get; set; }
        [JsonProperty("incomeID")]
        public long? IncomeID { get; set; }
        [JsonProperty("odid")]
        public long? Odid { get; set; }
        [JsonProperty("nmId")]
        public long? NmId { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("isCancel")]
        public bool IsCancel { get; set; }
        [JsonProperty("cancel_dt")]
        public DateTime CancelDT { get; set; }
        [JsonProperty("gNumber")]
        public string GNumber { get; set; }
        [JsonProperty("sticker")]
        public string Sticker { get; set; }
    }

}
