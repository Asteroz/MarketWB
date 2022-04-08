using MarketAI.API.Models;
using MarketWB.Parsing.Models;
using MarketWB.Parsing.Models.Reports;
using MarketWB.Parsing.Models.Reports.Orders;
using MarketWB.Parsing.Models.Reports.Rejects;
using MarketWB.Parsing.Models.Reports.Returns;
using MarketWB.Parsing.Models.Reports.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildberriesAPI;

namespace MarketWB.Parsing
{
    public static class MarketWBParser
    {
        public static async Task<SalesReport> GenerateSalesReport(DateTime from,DateTime to, WBAPITokenModel apikey)
        {
            var report = new SalesReport();
            var wbapi = new Wildberries(apikey.APIKey);

            var sales = await wbapi.GetSales(from);
            var realizations = await wbapi.GetReportDetailByPeriod(from,to);

            foreach (var realization in realizations)
            {
                var selfCost = apikey.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);
                double profit = (double)realization.ppvz_for_pay;
                if (selfCost != null)
                    profit -= selfCost.TotalSelfCost;

                var sale = sales.FirstOrDefault(o => o.NmId == realization.NmId);

                var salesRow = new SalesReportRow
                {
                    ThumbnailPath = null,
                    SaleDate = realization.SaleDt,
                    Category = sale?.Category,
                    Price = realization.ppvz_for_pay,
                    Profit = profit,
                    Title = realization.SubjectName,
                    DeliveryAddress = realization.OfficeName,
                    OrderDate = realization.OrderDt,
                    Logistic = realization.DeliveryRub,               
                };
                report.Rows.Add(salesRow);
            }
            return report;
        }
        public static async Task<OrdersReport> GenerateOrdersReport(DateTime from, DateTime to, WBAPITokenModel apikey)
        {
            var report = new OrdersReport();
            var wbapi = new Wildberries(apikey.APIKey);

            var orders = await wbapi.GetOrders(from);
            var realizations = await wbapi.GetReportDetailByPeriod(from,to);
            foreach (var realization in realizations)
            {
                var order = orders.FirstOrDefault(o => o.NmId == realization.NmId);

                var salesRow = new OrdersReportRow
                {
                    ThumbnailPath = null,
                    Category = order?.Category,
                    Price = realization.ppvz_for_pay,
                    OrderDate = realization.OrderDt,
                    Title = realization.SubjectName,
                    DeliveryAddress = realization.OfficeName,
                    Logistic = realization.DeliveryRub,
                };
                report.Rows.Add(salesRow);
            }

            return report;
        }
        public static async Task<RejectsReport> GenerateRejectsReport(DateTime from, DateTime to, WBAPITokenModel apikey)
        {
            var report = new RejectsReport();
            var wbapi = new Wildberries(apikey.APIKey);

            var orders = await wbapi.GetOrders(from);
            var realizations = await wbapi.GetReportDetailByPeriod(from, to);

            foreach (var order in orders.Where(o => o.IsCancel))
            {
                var realization = realizations.FirstOrDefault(o => o.NmId ==order.NmId);

                var selfCost = apikey.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);
                double profit = (double)realization.ppvz_for_pay;
                if (selfCost != null)
                    profit -= selfCost.TotalSelfCost;

                var salesRow = new RejectsReportRow
                {
                    ThumbnailPath = null,
                    Category = order.Category,
                    Price = order.TotalPrice,
                    SaleDate = realization.SaleDt,
                    OrderDate = order.Date,
                    DeliveryAddress = realization.OfficeName,
                    Logistic = realization.DeliveryRub,
                    Title = realization.SubjectName,
                    LostProfit = profit,                 
                };
                report.Rows.Add(salesRow);
            }

            return report;
        }
        public static async Task<ReturnsReport> GenerateReturnsReport(DateTime from, DateTime to, WBAPITokenModel apikey)
        {
            var report = new ReturnsReport();
            var wbapi = new Wildberries(apikey.APIKey);


            var orders = await wbapi.GetOrders(from);
            var realizations = await wbapi.GetReportDetailByPeriod(from, to);
            foreach (var realization in realizations)
            {
                var order = orders.FirstOrDefault(o => o.NmId == realization.NmId);

                var selfCost = apikey.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);
                double profit = (double)realization.ppvz_for_pay;
                if (selfCost != null)
                    profit -= selfCost.TotalSelfCost;

                var salesRow = new ReturnsReportRow
                {
                    ThumbnailPath = null,
                    SaleDate = realization.SaleDt,
                    OrderDate = realization.OrderDt,
                    DeliveryAddress = realization.OfficeName,
                    Logistic = realization.DeliveryRub,
                    Title = realization.SubjectName,
                    Category = order?.Category,
                    Price = realization.ppvz_for_pay,
                    Profit = profit,
                };
                report.Rows.Add(salesRow);
            }

            return report;
        }

        public static async Task<DashboardReport> GenerateDashboardReport(DateTime from, DateTime to, WBAPITokenModel apikey)
        {
            DashboardReport report = new DashboardReport();


            return report;
        }
    }
}
