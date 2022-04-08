using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Sales
{
    public class SalesReport
    {
        public List<SalesReportRow> Rows { get; set; } = new List<SalesReportRow>();
    }
}
