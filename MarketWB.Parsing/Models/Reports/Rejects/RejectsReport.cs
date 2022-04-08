using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Rejects
{
    public class RejectsReport
    {
        public List<RejectsReportRow> Rows { get; set; } = new List<RejectsReportRow>();
    }
}
