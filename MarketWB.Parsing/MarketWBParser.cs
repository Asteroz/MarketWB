using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.WB;
using MarketWB.Parsing.Models;
using MarketWB.Parsing.Models.Reports;
using MarketWB.Parsing.Models.Reports.Orders;
using MarketWB.Parsing.Models.Reports.Rejects;
using MarketWB.Parsing.Models.Reports.Returns;
using MarketWB.Parsing.Models.Reports.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildberriesAPI;
using WildberriesAPI.Models;

namespace MarketWB.Parsing
{
    public static class MarketWBParser
    {

        public static APIDBContext db { get; set; } = new APIDBContext();

        public static async Task<SalesReport> GenerateSalesReport(UserData filter)
        {
            var report = new SalesReport();
            if (filter.SelectedWBAPIToken is null) return report;

            List<WBSaleModel> sales = new List<WBSaleModel>();
            List<DetailByPeriodModel> realizations = new List<DetailByPeriodModel>();

            sales = await db.WBSales.Where(o => o.Date >= filter.SelectedPeriodFrom && o.Date <= filter.SelectedPeriodTo
                        && o.APIKey == filter.SelectedWBAPIToken.APIKey).ToListAsync();
            realizations = await db.DetailByPeriodModels
                .Where(o =>
                o.RrDt >= filter.SelectedPeriodFrom
                && o.RrDt <= filter.SelectedPeriodTo
                && o.APIKey == filter.SelectedWBAPIToken.APIKey)
                .ToListAsync();
            sales = await FilterSales(filter, sales);
            realizations = await FilterRealizations(filter, realizations);

            foreach (var realization in realizations)
            {
                var selfCost = filter.SelectedWBAPIToken.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);
                var sale = sales.FirstOrDefault(o => o.NmId == realization.NmId);

                if (sale is null) continue;


                var salesRow = new SalesReportRow
                {
                    ThumbnailPath = null,
                    SaleDate = realization.SaleDt,
                    Category = sale?.Category,
                    Price = realization.RetailPrice,
                    Title = realization.SubjectName,
                    DeliveryAddress = realization.OfficeName,
                    OrderDate = realization.OrderDt,
                    Logistic = realization.DeliveryRub,

                    Brand = realization.BrandName
                };

                double profit = 0;
                if (realization.DeliveryRub > 0)
                {
                    salesRow.Price = sale.FinishedPrice;
                    profit = (double)sale.ForPay;
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                else
                {
                    profit = (double)realization.ppvz_for_pay;
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                salesRow.Profit = profit;

                report.Rows.Add(salesRow);
            }


            return report;
        }
        public static async Task<OrdersReport> GenerateOrdersReport(UserData filter)
        {
            var report = new OrdersReport();
            if (filter.SelectedWBAPIToken is null) return report;

            List<WBOrderModel> orders = new List<WBOrderModel>();
            List<DetailByPeriodModel> realizations = new List<DetailByPeriodModel>();

            orders = await db.WBOrders.Where(o => o.Date >= filter.SelectedPeriodFrom && o.Date
                        <= filter.SelectedPeriodTo && o.APIKey == filter.SelectedWBAPIToken.APIKey).ToListAsync();
            realizations = await db.DetailByPeriodModels
                   .Where(o =>
                   o.RrDt >= filter.SelectedPeriodFrom
                   && o.RrDt <= filter.SelectedPeriodTo
                   && o.APIKey == filter.SelectedWBAPIToken.APIKey)
                   .ToListAsync();
            orders = await FilterOrders(filter, orders);
            realizations = await FilterRealizations(filter, realizations);


            foreach (var realization in realizations)
            {
                var order = orders.FirstOrDefault(o => o.NmId == realization.NmId);

                var salesRow = new OrdersReportRow
                {
                    ThumbnailPath = null,
                    Category = order?.Category,
                    OrderDate = realization.OrderDt,
                    Title = realization.SubjectName,
                    DeliveryAddress = realization.OfficeName,
                    Logistic = realization.DeliveryRub,
                };
                if (realization.DeliveryRub > 0)
                {
                    salesRow.Price = order.TotalPrice;
                }
                else
                {
                    salesRow.Price = realization.ppvz_for_pay;
                }

                report.Rows.Add(salesRow);
            }

            return report;
        }
        public static async Task<RejectsReport> GenerateRejectsReport(UserData filter)
        {           
            var report = new RejectsReport();
            if (filter.SelectedWBAPIToken is null) return report;

            List<WBOrderModel> orders;
            List<DetailByPeriodModel> realizations;

            orders = db.WBOrders.Where(o => o.Date >= filter.SelectedPeriodFrom && o.Date <= filter.SelectedPeriodTo
                                   && o.APIKey == filter.SelectedWBAPIToken.APIKey).ToList();
            realizations = db.DetailByPeriodModels
                   .Where(o =>
                   o.RrDt >= filter.SelectedPeriodFrom
                   && o.RrDt <= filter.SelectedPeriodTo
                   && o.APIKey == filter.SelectedWBAPIToken.APIKey).ToList();
            orders = await FilterOrders(filter, orders);
            realizations = await FilterRealizations(filter, realizations);

            foreach (var order in orders.Where(o => o.IsCancel))
            {
                var realization = realizations.FirstOrDefault(o => o.NmId == order.NmId);
                if (realization == null) continue;

                SelfCostModel selfCost = filter.SelectedWBAPIToken.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);

                var salesRow = new RejectsReportRow
                {
                    ThumbnailPath = null,
                    Category = order.Category,
                    SaleDate = realization?.SaleDt,
                    OrderDate = order.Date,
                    DeliveryAddress = realization?.OfficeName,
                    Logistic = realization?.DeliveryRub,
                    Title = realization?.SubjectName,
                };


                double profit = 0;
                if (realization.DeliveryRub > 0)
                {
                    salesRow.Price = order.TotalPrice;
                    profit = (double)order.TotalPrice - ((double)order.TotalPrice / 100 * (double)order.DiscountPercent);
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                else
                {
                    salesRow.Price = realization.RetailPrice;
                    profit = (double)realization.ppvz_for_pay;
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                if (selfCost != null)
                    profit -= selfCost.TotalSelfCost;

                salesRow.LostProfit = profit;


                report.Rows.Add(salesRow);
            }
            return report;
        }
        public static async Task<ReturnsReport> GenerateReturnsReport(UserData filter)
        {
            var report = new ReturnsReport();
            if (filter.SelectedWBAPIToken is null) return report;

            List<WBOrderModel> orders = new List<WBOrderModel>();
            List<DetailByPeriodModel> realizations = new List<DetailByPeriodModel>();

            orders = await db.WBOrders.Where(o => o.Date >= filter.SelectedPeriodFrom && o.Date <= filter.SelectedPeriodTo
                                                          && o.APIKey == filter.SelectedWBAPIToken.APIKey).ToListAsync();
            realizations = await db.DetailByPeriodModels
                .Where(o =>
                o.RrDt >= filter.SelectedPeriodFrom
                && o.RrDt <= filter.SelectedPeriodTo
                && o.APIKey == filter.SelectedWBAPIToken.APIKey)
                .ToListAsync();
            orders = await FilterOrders(filter, orders);
            realizations = await FilterRealizations(filter, realizations);

            foreach (var realization in realizations.Where(o => o.ReturnAmount > 0))
            {
                var order = orders.FirstOrDefault(o => o.NmId == realization.NmId);
                if (order is null) continue;


                var selfCost = filter.SelectedWBAPIToken.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);

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
                };

                double profit = 0;
                if (realization.DeliveryRub > 0)
                {
                    salesRow.Price = order.TotalPrice;
                    profit = (double)order.TotalPrice - ((double)order.TotalPrice / 100 * (double)order.DiscountPercent);
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                else
                {
                    profit = (double)realization.ppvz_for_pay;
                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;
                }
                salesRow.Profit = profit;

                report.Rows.Add(salesRow);
            }
            return report;
        }

        public static async Task<DashboardReport> GenerateDashboardReport(UserData filter)
        {
            if (filter.SelectedWBAPIToken is null)
                return new DashboardReport();

            var sharedFilter = new UserData
            {
                SelectedPeriodFrom = filter.SelectedPeriodFrom,
                SelectedPeriodTo = filter.SelectedPeriodTo,
                SelectedWBAPIToken = filter.SelectedWBAPIToken,
                SelectedWBCategory = "",
                SelectedWBBrand = "",
            };

            var allSales = await GenerateSalesReport(sharedFilter);

            var sales = await GenerateSalesReport(filter);
            var cancels = await GenerateRejectsReport(filter);
            var returns = await GenerateReturnsReport(filter);
            var orders = await GenerateOrdersReport(filter);

            double total = 0;
            double profit = 0;
            double marginality = 0;

            foreach(var sale in sales.Rows)
            {
                profit += (double)sale.Profit;
                total += (double)sale.Price;
            }
            double percent = total / 100;
            marginality = profit / percent;

            DashboardReport report = new DashboardReport()
            {
                From = filter.SelectedPeriodFrom,
                To = filter.SelectedPeriodTo,

                CancelsCount = cancels.Rows.Count,
                CancelsSum = Math.Round((double)cancels.Rows.Sum(o => o.Price),2),

                OrdersCount = orders.Rows.Count,
                OrdersSum = Math.Round((double)orders.Rows.Sum(o => o.Price),2),

                ReturnsCount = returns.Rows.Count,
                ReturnsSum = Math.Round((double)returns.Rows.Sum(o => o.Price),2),

                SalesCount = sales.Rows.Count,
                SalesSum = Math.Round((double)sales.Rows.Sum(o => o.Price),2),

                SalesCountToday = sales.Rows.Count(o => o.SaleDate >= DateTime.UtcNow.Date),
                SalesSumToday = Math.Round((double)sales.Rows.Where(o => o.SaleDate >= DateTime.UtcNow.Date).Sum(o => o.Price),2),
                ReturnsCountToday = returns.Rows.Count(o => o.SaleDate >= DateTime.UtcNow.Date),
                ReturnsSumToday = Math.Round((double)returns.Rows.Where(o => o.SaleDate >= DateTime.UtcNow.Date).Sum(o => o.Price),2),

                Marginality = Math.Round(marginality,2),
                Profit = Math.Round(profit,2)
            };

            foreach(var grouping in allSales.Rows.GroupBy(o => o.Title))
            {
                double totalInGroup = 0;
                double profitInGroup = 0;
                double marginalityInGroup = 0;
                foreach (var sale in grouping)
                {
                    totalInGroup += (double)sale.Price;
                    profitInGroup += (double)sale.Profit;
                }
                double percentInGroup = totalInGroup / 100;
                marginalityInGroup = profitInGroup / percentInGroup;

                report.MarginalityByCategories.Add(grouping.Key, Math.Round(marginalityInGroup,2));
                report.ProfitByCategories.Add(grouping.Key, Math.Round(profitInGroup,2));
            }
            foreach (var grouping in allSales.Rows.GroupBy(o => o.Brand))
            {
                double totalInGroup = 0;
                double profitInGroup = 0;
                double marginalityInGroup = 0;
                foreach (var sale in grouping)
                {
                    totalInGroup += (double)sale.Price;
                    profitInGroup += (double)sale.Profit;
                }
                double percentInGroup = totalInGroup / 100;
                marginalityInGroup = profitInGroup / percentInGroup;

                report.MarginalityByBrands.Add(grouping.Key, Math.Round(marginalityInGroup,2));
                report.ProfitByBrands.Add(grouping.Key, Math.Round(profitInGroup,2));
            }
            return report;
        }

        public static async Task<List<AvailableWBCategory>> GetWBCategories(WBAPITokenModel apikey)
        {
            if (apikey is null) return new List<AvailableWBCategory>();
            return await db.AvailableWBCategories.Where(o => o.APIKey == apikey.APIKey).ToListAsync();
        }
        public static async Task<List<AvailableWBBrand>> GetWBBrands(WBAPITokenModel apikey)
        {
            if (apikey is null) return new List<AvailableWBBrand>();
            using (APIDBContext db = new APIDBContext())
                return await db.AvailableWBBrands.Where(o => o.APIKey == apikey.APIKey).ToListAsync();
        }



        private static async Task<List<WBSaleModel>> FilterSales(UserData filter, List<WBSaleModel> sales)
        {
            if (await HasAPIKeyBrand(filter.SelectedWBAPIToken.APIKey,filter.SelectedWBBrand))
            {
                sales = sales.Where(o => o.Brand == filter.SelectedWBBrand).ToList();
            }
            if (await HasAPIKeyCategory(filter.SelectedWBAPIToken.APIKey, filter.SelectedWBCategory))
            {
                sales = sales.Where(o => o.Subject == filter.SelectedWBCategory).ToList();
            }
            return sales;
        }
        private static async Task<List<WBOrderModel>> FilterOrders(UserData filter, List<WBOrderModel> orders)
        {
            if (await HasAPIKeyBrand(filter.SelectedWBAPIToken.APIKey, filter.SelectedWBBrand))
            {
                orders = orders.Where(o => o.Brand == filter.SelectedWBBrand).ToList();
            }
            if (await HasAPIKeyCategory(filter.SelectedWBAPIToken.APIKey, filter.SelectedWBCategory))
            {
                orders = orders.Where(o => o.Subject == filter.SelectedWBCategory).ToList();
            }
            return orders;
        }
        private static async Task<List<DetailByPeriodModel>> FilterRealizations(UserData filter, List<DetailByPeriodModel> realizations)
        {
            if (await HasAPIKeyBrand(filter.SelectedWBAPIToken.APIKey, filter.SelectedWBBrand))
            {
                realizations = realizations.Where(o => o.BrandName == filter.SelectedWBBrand).ToList();
            }
            if (await HasAPIKeyCategory(filter.SelectedWBAPIToken.APIKey, filter.SelectedWBCategory))
            {
                realizations = realizations.Where(o => o.SubjectName == filter.SelectedWBCategory).ToList();
            }
            return realizations;
        }


        private static async Task<bool> HasAPIKeyBrand(string apikey,string brand)
        {
            if (string.IsNullOrEmpty(brand)) return false;
            if (string.IsNullOrEmpty(apikey)) return false;
            if (string.IsNullOrEmpty("Все бренды")) return false;
            return db.AvailableWBBrands.Any(o => o.APIKey == apikey && o.Brand == brand);
        }
        private static async Task<bool> HasAPIKeyCategory(string apikey, string category)
        {
            if (string.IsNullOrEmpty(category)) return false;
            if (string.IsNullOrEmpty(apikey)) return false;
            if (string.IsNullOrEmpty("Все категории")) return false;
            return db.AvailableWBCategories.Any(o => o.APIKey == apikey && o.Category == category);
        }
    }
}
