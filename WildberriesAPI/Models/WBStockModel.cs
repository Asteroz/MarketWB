using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WildberriesAPI.Models
{
    public class WBStockModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string APIKey { get; set; }


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
        [JsonProperty("isSupply")]
        public bool IsSupply { get; set; }
        [JsonProperty("isRealization")]
        public bool IsRealization { get; set; }
        [JsonProperty("quantityFull")]
        public long? QuantityFull { get; set; }
        [JsonProperty("quantityNotInOrders")]
        public long? QuantityNotInOrders { get; set; }
        [JsonProperty("warehouse")]
        public long? Warehouse { get; set; }
        [JsonProperty("warehouseName")]
        public string WarehouseName { get; set; }
        [JsonProperty("inWayToClient")]
        public long? InWayToClient { get; set; }
        [JsonProperty("inWayFromClient")]
        public long? InWayFromClient { get; set; }
        [JsonProperty("nmId")]
        public long? NmId { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("daysOnSite")]
        public long? DaysOnSite { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("SCCode")]
        public string SCCode { get; set; }
        [JsonProperty("Price")]
        public double? Price { get; set; }
        [JsonProperty("Discount")]
        public double? Discount { get; set; }
    }
}
