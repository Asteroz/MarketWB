using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models
{
    public class ReportFilter
    { 
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
    }
}
