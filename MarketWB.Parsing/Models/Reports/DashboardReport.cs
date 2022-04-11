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
        public double Marginality { get; set; }


        public int SalesCountToday { get; set; }
        public double SalesSumToday { get; set; }
        public int ReturnsCountToday { get; set; }
        public double ReturnsSumToday { get; set; }


        public Dictionary<string,double> MarginalityByCategories { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> ProfitByCategories { get; set; } = new Dictionary<string, double>();

        public Dictionary<string, double> MarginalityByBrands { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> ProfitByBrands { get; set; } = new Dictionary<string, double>();
    }
}
