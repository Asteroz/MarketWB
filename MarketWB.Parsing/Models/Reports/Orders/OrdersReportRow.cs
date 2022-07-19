using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Orders
{
    public class OrdersReportRow
    {
        public string ThumbnailPath { get; set; }

        public long? Odid { get; set; }

        public bool IsSelfBuy { get; set; }

        public string Brand { get; set; }
        public long Article { get; set; }

        public string Title { get; set; }
        public double? Price { get; set; }
        public string Category { get; set; }

        public double? Logistic { get; set; }
        public double? SelfCost { get; set; }

        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
