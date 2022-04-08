using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Orders
{
    public class OrdersReport
    {
        public List<OrdersReportRow> Rows { get; set; } = new List<OrdersReportRow>();
    }
}
