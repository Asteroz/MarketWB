using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports
{
    public class DashboardReport
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int SalesCount { get; set; }
        public double SalesSum { get; set; }


        public int OrdersCount { get; set; }
        public double OrdersSum { get; set; }


        public int ReturnsCount { get; set; }
        public double ReturnsSum { get; set; }


        public int CancelsCount { get; set; }
        public double CancelsSum { get; set; }


        public double Profit { get; set; }
    }
}
