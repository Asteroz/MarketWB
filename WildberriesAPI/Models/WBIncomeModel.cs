using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WildberriesAPI.Models
{
    public class WBIncomeModel
    {
        [JsonProperty("incomeId")]
        public long? IncomeId { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
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
        [JsonProperty("dateClose")]
        public DateTime DateClose { get; set; }
        [JsonProperty("warehouseName")]
        public string WarehouseName { get; set; }
        [JsonProperty("nmId")]
        public long? NmId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
