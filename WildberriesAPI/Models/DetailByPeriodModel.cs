using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WildberriesAPI.Models
{
    public class DetailByPeriodModel
    {
        [JsonProperty("realizationreport_id")]
        public long? RealizationReportId { get; set; }
        [JsonProperty("suppliercontract_code")]
        public string SupplierContractCode { get; set; }
        [JsonProperty("rrd_id")]
        public long? RrdId { get; set; }
        [JsonProperty("gi_id")]
        public long? GiId { get; set; }
        [JsonProperty("subject_name")]
        public string SubjectName { get; set; }
        [JsonProperty("nm_id")]
        public long? NmId { get; set; }
        [JsonProperty("brand_name")]
        public string BrandName { get; set; }
        [JsonProperty("sa_name")]
        public string SaName { get; set; }
        [JsonProperty("ts_name")]
        public string TsName { get; set; }
        [JsonProperty("barcode")]
        public string Barcode { get; set; }
        [JsonProperty("doc_type_name")]
        public string DocTypeName { get; set; }
        [JsonProperty("quantity")]
        public long? Quantity { get; set; }
        [JsonProperty("retail_price")]
        public double? RetailPrice { get; set; }
        [JsonProperty("retail_amount")]
        public double? RetailAmount { get; set; }
        [JsonProperty("sale_percent")]
        public double? SalePercent { get; set; }
        [JsonProperty("commission_percent")]
        public double? CommissionPercent { get; set; }
        [JsonProperty("office_name")]
        public string OfficeName { get; set; }
        [JsonProperty("supplier_oper_name")]
        public string SupplierOperName { get; set; }
        [JsonProperty("order_dt")]
        public DateTime OrderDt { get; set; }
        [JsonProperty("sale_dt")]
        public DateTime SaleDt { get; set; }
        [JsonProperty("rr_dt")]
        public DateTime RrDt { get; set; }
        [JsonProperty("shk_id")]
        public long? ShkId { get; set; }
        [JsonProperty("retail_price_withdisc_rub")]
        public double? RetailPriceWithdiscRub { get; set; }
        [JsonProperty("delivery_amount")]
        public long? DeliveryAmount { get; set; }
        [JsonProperty("return_amount")]
        public long? ReturnAmount { get; set; }
        [JsonProperty("delivery_rub")]
        public double? DeliveryRub { get; set; }
        [JsonProperty("gi_box_type_name")]
        public string GiBoxTypeName { get; set; }
        [JsonProperty("product_discount_for_report")]
        public double? ProductDiscountForReport { get; set; }
        [JsonProperty("supplier_promo")]
        public double? SupplierPromo { get; set; }
        [JsonProperty("rid")]
        public long? Rid { get; set; }
        public double? ppvz_spp_prc { get; set; }
        public double? ppvz_kvw_prc_base { get; set; }
        public double? ppvz_kvw_prc { get; set; }
        public double? ppvz_sales_commission { get; set; }
        public double? ppvz_for_pay { get; set; }
        public double? ppvz_reward { get; set; }
        public double? ppvz_vw { get; set; }
        public double? ppvz_vw_nds { get; set; }
        public double? ppvz_office_id { get; set; }
        public string ppvz_office_name { get; set; }
        public long? ppvz_supplier_id { get; set; }
        [JsonProperty("declaration_number")]
        public string DeclarationNumber { get; set; }
        [JsonProperty("sticker_id")]
        public long? StickerId { get; set; }
    }



}
