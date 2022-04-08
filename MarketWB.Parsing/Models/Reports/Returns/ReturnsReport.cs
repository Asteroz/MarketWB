using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Returns
{
    public class ReturnsReport
    {
        public List<ReturnsReportRow> Rows { get; set; } = new List<ReturnsReportRow>();
    }
}
