﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketWB.Parsing.Models.Reports.Returns
{
    public class ReturnsReportRow
    {
        public string ThumbnailPath { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public string Category { get; set; }
        public double? Profit { get; set; }
        public double? Logistic { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime SaleDate { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
