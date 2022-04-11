using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WildberriesAPI.Models
{
    public class WBSaleModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string APIKey { get; set; }


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
        [JsonProperty("discountPercent")]
        public double? DiscountPercent { get; set; }
        [JsonProperty("isSupply")]
        public bool IsSupply { get; set; }
        [JsonProperty("isRealization")]
        public bool IsRealization { get; set; }
        [JsonProperty("orderId")]
        public long? OrderId { get; set; }
        [JsonProperty("promoCodeDiscount")]
        public double PromoCodeDiscount { get; set; }
        [JsonProperty("warehouseName")]
        public string WarehouseName { get; set; }
        [JsonProperty("countryName")]
        public string CountryName { get; set; }
        [JsonProperty("oblastOkrugName")]
        public string OblastOkrugName { get; set; }
        [JsonProperty("regionName")]
        public string RegionName { get; set; }
        [JsonProperty("incomeID")]
        public long? IncomeID { get; set; }
        [JsonProperty("saleID")]
        public string SaleID { get; set; }
        [JsonProperty("odid")]
        public long? Odid { get; set; }
        [JsonProperty("spp")]
        public double? Spp { get; set; }
        [JsonProperty("forPay")]
        public double? ForPay { get; set; }
        [JsonProperty("finishedPrice")]
        public double? FinishedPrice { get; set; }
        [JsonProperty("priceWithDisc")]
        public double? PriceWithDisc { get; set; }
        [JsonProperty("nmId")]
        public long? NmId { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("IsStorno")]
        public long? IsStorno { get; set; }
        [JsonProperty("gNumber")]
        public string GNumber { get; set; }
        [JsonProperty("sticker")]
        public string Sticker { get; set; }
    }

}
