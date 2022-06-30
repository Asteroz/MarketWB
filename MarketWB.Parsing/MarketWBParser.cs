using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.WB;
using MarketWB.Parsing.Models;
using MarketWB.Parsing.Models.Misc;
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
using WildberriesAPI.Comparers;
using WildberriesAPI.Models;

namespace MarketWB.Parsing
{
    public class MarketWBParser
    {

        private readonly DateTimeComparer dateTimeComparer = new DateTimeComparer();
        private readonly APIDBContext db;
        public MarketWBParser(APIDBContext _db)
        {
            db = _db;
        }

        public SalesReport GenerateSalesReport(UserModel user)
        {
            var report = new SalesReport();
            if (user.UserData.SelectedWBAPITokens.Count == 0) return report;

            foreach(var token in user.UserData.SelectedWBAPITokens)
            {
                List<WBSaleModel> sales = new List<WBSaleModel>();
                List<DetailByPeriodModel> realizations = new List<DetailByPeriodModel>();

                sales = db.WBSales
                    .Where(o => o.Date >= user.UserData.SelectedPeriodFrom
                     && o.Date <= user.UserData.SelectedPeriodTo
                     && o.APIKey == token.APIKey)
                    .Where(o => o.FinishedPrice > 0)
                    .ToList();

                var orders = db.WBOrders
                 .Where(o => o.APIKey == token.APIKey)
                 .ToList();

                realizations = db.DetailByPeriodModels.
                     Where(o => o.APIKey == token.APIKey)
                    .ToList();

                sales = FilterSales(user.UserData, sales);
                realizations = FilterRealizations(user.UserData, realizations);

                foreach (var sale in sales)
                {
                    var selfCost = user.SelfCosts.FirstOrDefault(o => o.ProductId == sale.NmId);
                    var realization = realizations.LastOrDefault(o => o.NmId == sale.NmId && o.DeliveryRub > 0 && o.ReturnAmount == 0);
                    var realization2 = realizations.LastOrDefault(o => o.NmId == sale.NmId && o.DeliveryRub == 0/* && o.ReturnAmount == 0*/);
                    if (realization == null && realization2 == null) 
                        continue;

                    var order = orders.FirstOrDefault(o => o.GNumber == sale.GNumber);

                    var salesRow = new SalesReportRow
                    {
                        ThumbnailPath = null,
                        SaleDate = sale.Date,
                        Category = sale.Category,
                        Title = sale.Subject,
                        DeliveryAddress = realization?.OfficeName,
                        OrderDate = order?.Date,

                        SelfCost = selfCost?.TotalSelfCost,

                        Logistic = realization?.DeliveryRub,

                        Brand = sale.Brand,
                        Article = (long)sale.NmId,

                        IsSelfBuy = sale.IsSelfBuy
                    };

                    double profit = 0;
                    salesRow.Price = Math.Abs((double)sale.FinishedPrice);
                    salesRow.Comission = realization2?.ppvz_sales_commission;



                    if (!sale.IsSelfBuy)
                        profit = Math.Abs((double)sale.FinishedPrice);

                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;

                    if (salesRow.Comission == null) salesRow.Comission = 0;
                    if (salesRow.Logistic == null) salesRow.Logistic = 0;

                    profit -= (double)salesRow.Comission;
                    profit -= (double)salesRow.Logistic;

                    salesRow.Profit = profit;

                    if (report.Rows.Any(o => o.SaleDate == sale.Date)) continue;
                    if (report.Rows.Any(o => o.OrderDate == salesRow.OrderDate)) continue;

                    report.Rows.Add(salesRow);
                }
            }
            return report;
        }

        public OrdersReport GenerateOrdersReport(UserModel user)

        {
            var report = new OrdersReport();
            if (user.UserData.SelectedWBAPITokens.Count == 0) return report;

            foreach (var token in user.UserData.SelectedWBAPITokens)
            {
                List<WBOrderModel> orders = new List<WBOrderModel>();
                List<DetailByPeriodModel> realizations = new List<DetailByPeriodModel>();

                orders = db.WBOrders.Where(o =>
                           o.Date >= user.UserData.SelectedPeriodFrom
                           && o.Date <= user.UserData.SelectedPeriodTo
                           && o.APIKey == token.APIKey)
                            .ToList();

                realizations = db.DetailByPeriodModels
                        .Where(o => o.APIKey == token.APIKey)
                        .ToList();

                orders = FilterOrders(user.UserData, orders);
                realizations = FilterRealizations(user.UserData, realizations);




                foreach (var order in orders)
                {
                    var realization = realizations.LastOrDefault(o => o.NmId == order.NmId && o.DeliveryRub > 0 && o.ReturnAmount == 0);
                    var selfCost = user.SelfCosts.FirstOrDefault(o => o.ProductId == order.NmId);

                    var salesRow = new OrdersReportRow
                    {
                        ThumbnailPath = null,
                        Category = order?.Category,
                        OrderDate = order.Date,
                        Title = order.Subject,
     
                        SelfCost = selfCost?.TotalSelfCost,

                        Brand = order.Brand,
                        Article = (long)order.NmId,

                        IsSelfBuy = order.IsSelfBuy
                    };

                    if(realization != null)
                    {
                        salesRow.DeliveryAddress = realization.OfficeName;
                        salesRow.Logistic = realization.DeliveryRub;
                    }

                    salesRow.Price = order.TotalPrice - (order.TotalPrice / 100 * order.DiscountPercent);

                    if (report.Rows.Any(o => o.OrderDate == order.Date)) continue;

                    report.Rows.Add(salesRow);
                }
            }
            return report;
        }


        public RejectsReport GenerateRejectsReport(UserModel user)
        {           
            var report = new RejectsReport();
            if (user.UserData.SelectedWBAPITokens.Count == 0) return report;

            foreach (var token in user.UserData.SelectedWBAPITokens)
            {
                var sales = db.WBSales.Where(o => o.APIKey == token.APIKey)
                       .ToList();

                List<WBOrderModel> orders = db.WBOrders.Where(o => 
                o.CancelDT >= user.UserData.SelectedPeriodFrom 
                && o.CancelDT <= user.UserData.SelectedPeriodTo
                && o.IsCancel
                && o.APIKey == token.APIKey)
                .ToList();

                List<DetailByPeriodModel> realizations = db.DetailByPeriodModels
                       .Where(o =>
                        o.APIKey == token.APIKey)
                       .ToList();

                orders = FilterOrders(user.UserData, orders).ToList();



                realizations = FilterRealizations(user.UserData, realizations);

                foreach (var order in orders)
                {
                    var realization = realizations.LastOrDefault(o => o.SaName == order.SupplierArticle && o.DeliveryRub == 0);
                    var realization2 = realizations.LastOrDefault(o => o.SaName == order.SupplierArticle && o.DeliveryRub != 0 && o.ReturnAmount == 0);
                    var realizationBack = realizations.LastOrDefault(o => o.SaName == order.SupplierArticle && o.DeliveryRub != 0 && o.ReturnAmount > 0);

                    if (realizationBack == null || realization2 == null) continue;

                    var sale = sales.FirstOrDefault(o => o.GNumber == order.GNumber);

                    SelfCostModel selfCost = user.SelfCosts.FirstOrDefault(o => o.ProductId == order.NmId);

                    var salesRow = new RejectsReportRow
                    {
                        ThumbnailPath = null,
                        Category = order.Category,
                        SaleDate = sale?.Date,
                        OrderDate = order.Date,
                        Title = order.Subject,
                        CancelDate = order.CancelDT,

                        Logistic = (double)realization2?.DeliveryRub + (double)realizationBack?.DeliveryRub,
                        LogisticFromClient = (double)realization2?.DeliveryRub,
                        LogisticToClient = (double)realizationBack?.DeliveryRub,

                        DeliveryAddress = realization2?.OfficeName,

                        Brand = order.Brand,
                        Article = (long)order.NmId
                    };


                    double profit = 0;

                    var price = (double)order.TotalPrice - ((double)order.TotalPrice / 100 * (double)order.DiscountPercent);

                    salesRow.Price = price;
                    profit = price;

                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;

                    salesRow.LostProfit = profit - salesRow.Logistic;
                    if (realization != null)
                        salesRow.LostProfit -= realization.ppvz_sales_commission;

                  //  if (report.Rows.Any(o => o.OrderDate == salesRow.OrderDate)) continue;
                    if (report.Rows.Any(o => o.CancelDate == order.CancelDT)) continue;

                    report.Rows.Add(salesRow);
                }
            }   
            return report;
        }
        public ReturnsReport GenerateReturnsReport(UserModel user)
        {
            var report = new ReturnsReport();
            if (user.UserData.SelectedWBAPITokens.Count == 0) return report;

            foreach (var token in user.UserData.SelectedWBAPITokens)
            {

                var stocks = db.WBStocks.Where(o => o.APIKey == token.APIKey).ToList();

               var notReturnedSales = db.WBSales.Where(o =>
                   o.APIKey == token.APIKey
                   && o.FinishedPrice > 0).ToList();

                var orders = db.WBOrders.Where(o => o.APIKey == token.APIKey).ToList();

                var sales = db.WBSales.Where(o => 
                    o.Date >= user.UserData.SelectedPeriodFrom.Date 
                    && o.Date <= user.UserData.SelectedPeriodTo                                             
                    && o.APIKey == token.APIKey
                    && o.FinishedPrice < 0
                    /*&& o.SaleID.StartsWith("R")*/).ToList();

           


                var realizations = db.DetailByPeriodModels
                    .Where(o =>  o.APIKey == token.APIKey)
                    .ToList();
                sales = FilterSales(user.UserData, sales);


                realizations = FilterRealizations(user.UserData, realizations);

                foreach (var sale in sales)
                {
                    var realization = realizations.LastOrDefault(o => o.SaName == sale.SupplierArticle && o.DeliveryRub > 0);
                    var realization2 = realizations.LastOrDefault(o => o.SaName == sale.SupplierArticle && o.DeliveryRub > 0 && o.ReturnAmount == 0);
                    var realizationBack = realizations.LastOrDefault(o => o.SaName == sale.SupplierArticle && o.DeliveryRub > 0 && o.ReturnAmount > 0);
                    var realizationComission = realizations.LastOrDefault(o => o.SaName == sale.SupplierArticle && o.DeliveryRub == 0);

                    if (realization == null || realization2 == null || realizationBack is null || realizationComission is null)
                        continue;

                   

                    var baseSale = notReturnedSales.FirstOrDefault(o => o.GNumber == sale.GNumber);

                    var order = orders.FirstOrDefault(o => o.GNumber == sale.GNumber);
                    if (order is null) continue;

                    var selfCost = user.SelfCosts.FirstOrDefault(o => o.ProductId == realization.NmId);

                    var salesRow = new ReturnsReportRow
                    {
                        ThumbnailPath = null,
                        SaleDate = baseSale?.Date,
                        OrderDate = order.Date,
                        DeliveryAddress = realization.OfficeName,

                        Logistic = (double)realization2.DeliveryRub + (double)realizationBack.DeliveryRub,
                        LogisticFromClient = (double)realization2.DeliveryRub,
                        LogisticToClient = (double)realizationBack.DeliveryRub,

                        Title = realization.SubjectName,
                        Category = sale?.Category,
                        Price = realization.ppvz_for_pay,
                        ReturnDate = sale.Date,

                        Brand = sale.Brand,
                        Article = (long)sale.NmId,
                    };

                    salesRow.InWayFromClient = (int)realizationBack.DeliveryRub;
                    salesRow.InWayToClient = (int)realization2.DeliveryRub;


                    var price = (double)sale.TotalPrice - ((double)sale.TotalPrice / 100 * (double)sale.DiscountPercent);
                    price = Math.Abs(price);
                    salesRow.Price = price;

                    double profit = price;

                    if (selfCost != null)
                        profit -= selfCost.TotalSelfCost;

                    profit -= (double)salesRow.Logistic;
                    profit -= (double)realizationComission.ppvz_sales_commission;

                    salesRow.Profit = profit;

                    if (report.Rows.Any(o => o.SaleDate == salesRow.SaleDate)) continue;

                    report.Rows.Add(salesRow);
                }
            }

          
            return report;
        }

        public DashboardReport GenerateDashboardReport(UserModel user)
        {
            if (user.UserData.SelectedWBAPITokens.Count == 0)
                return new DashboardReport();


            var sharedFilter = new UserData
            {
                SelectedPeriodFrom = user.UserData.SelectedPeriodFrom,
                SelectedPeriodTo = user.UserData.SelectedPeriodTo,
                SelectedWBAPITokens = user.UserData.SelectedWBAPITokens,
                SelectedWBCategory = "",
                SelectedWBBrand = "",
            };

            var filterUser = new UserModel();
            filterUser.UserData = sharedFilter;
           

            var allSales = GenerateSalesReport(filterUser);



            var sales = GenerateSalesReport(user);
            var cancels =  GenerateRejectsReport(user);
            var returns =  GenerateReturnsReport(user);
            var orders =  GenerateOrdersReport(user);





            double total = 0;
            double profit = 0;
            double marginality = 0;

            foreach(var sale in sales.Rows)
            {
                if(!sale.IsSelfBuy)
                    profit += (double)sale.Profit;

                total += (double)sale.Price;
            }
            double percent = total / 100;
            marginality = profit / percent;

            DashboardReport report = new DashboardReport()
            {
                From = user.UserData.SelectedPeriodFrom,
                To = user.UserData.SelectedPeriodTo,

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


            //report.LogisticFromClient += (double)sales.Rows.Sum(o => o.Logistic);
            //report.LogisticFromClient += (double)orders.Rows.Sum(o => o.Logistic);
            //report.LogisticFromClient += (double)returns.Rows.Sum(o => o.LogisticFromClient);
            //report.LogisticFromClient += (double)cancels.Rows.Sum(o => o.LogisticFromClient);

            //report.LogisticToClient += (double)returns.Rows.Sum(o => o.LogisticToClient);
            //report.LogisticToClient += (double)cancels.Rows.Sum(o => o.LogisticToClient);


            var logistic = GetLogistic(user);
            report.LogisticFromClient = logistic.FromClient;
            report.LogisticToClient = logistic.ToClient;


            foreach(var saleGroup in sales.Rows.GroupBy(o => o.SaleDate.Date))
            {
                report.SalesCountChartData.Add(new ChartDay
                {
                    Value = saleGroup.Count(),
                    Date = saleGroup.Key
                });
                report.SalesPriceChartData.Add(new ChartDay
                {
                    Value = (int)saleGroup.Sum(o => o.Price),
                    Date = saleGroup.Key
                });
            }

            return report;
        }




        public List<AvailableWBCategory> GetWBCategories(WBAPITokenModel apikey)
        {
            if (apikey is null) return new List<AvailableWBCategory>();
            return db.AvailableWBCategories.Where(o => o.APIKey == apikey.APIKey).ToList();
        }
        public List<AvailableWBCategory> GetWBCategories(List<WBAPITokenModel> apikeys)
        {
            List<AvailableWBCategory> items = new List<AvailableWBCategory>();
            foreach (var token in apikeys)
            {
                items.AddRange(db.AvailableWBCategories.Where(o => o.APIKey == token.APIKey).ToList());
            }
            return items;
        }




        public List<AvailableWBBrand> GetWBBrands(WBAPITokenModel apikey)
        {
            if (apikey is null) return new List<AvailableWBBrand>();
            return db.AvailableWBBrands.Where(o => o.APIKey == apikey.APIKey).ToList();
        }
        public List<AvailableWBBrand> GetWBBrands(List<WBAPITokenModel> apikeys)
        {
            List<AvailableWBBrand> items = new List<AvailableWBBrand>();
            foreach(var token in apikeys)
            {
                items.AddRange(db.AvailableWBBrands.Where(o => o.APIKey == token.APIKey).ToList());
            }
            return items;
        }


        private List<WBSaleModel> FilterSales(UserData filter, List<WBSaleModel> sales)
        {
            foreach(var token in filter.SelectedWBAPITokens)
            {
                if (HasAPIKeyBrand(token.APIKey, filter.SelectedWBBrand))
                {
                    sales = sales.Where(o => o.Brand == filter.SelectedWBBrand).ToList();
                }
                if (HasAPIKeyCategory(token.APIKey, filter.SelectedWBCategory))
                {
                    sales = sales.Where(o => o.Subject == filter.SelectedWBCategory).ToList();
                }
            }
           
            return sales;
        }
        private List<WBOrderModel> FilterOrders(UserData filter, List<WBOrderModel> orders)
        {
            foreach (var token in filter.SelectedWBAPITokens)
            {
                if (HasAPIKeyBrand(token.APIKey, filter.SelectedWBBrand))
                {
                    orders = orders.Where(o => o.Brand == filter.SelectedWBBrand).ToList();
                }
                if (HasAPIKeyCategory(token.APIKey, filter.SelectedWBCategory))
                {
                    orders = orders.Where(o => o.Subject == filter.SelectedWBCategory).ToList();
                }
            }         
            return orders;
        }
        private List<DetailByPeriodModel> FilterRealizations(UserData filter, List<DetailByPeriodModel> realizations)
        {
            foreach (var token in filter.SelectedWBAPITokens)
            {
                if (HasAPIKeyBrand(token.APIKey, filter.SelectedWBBrand))
                {
                    realizations = realizations.Where(o => o.BrandName == filter.SelectedWBBrand).ToList();
                }
                if (HasAPIKeyCategory(token.APIKey, filter.SelectedWBCategory))
                {
                    realizations = realizations.Where(o => o.SubjectName == filter.SelectedWBCategory).ToList();
                }
            }
            return realizations;
        }


        private bool HasAPIKeyBrand(string apikey,string brand)
        {
            if (string.IsNullOrEmpty(brand)) return false;
            if (string.IsNullOrEmpty(apikey)) return false;
            if (string.IsNullOrEmpty("Все бренды")) return false;
            return db.AvailableWBBrands.Any(o => o.APIKey == apikey && o.Brand == brand);
        }
        private bool HasAPIKeyCategory(string apikey, string category)
        {
            if (string.IsNullOrEmpty(category)) return false;
            if (string.IsNullOrEmpty(apikey)) return false;
            if (string.IsNullOrEmpty("Все категории")) return false;
            return db.AvailableWBCategories.Any(o => o.APIKey == apikey && o.Category == category);
        }


        private LogisticInfo GetLogistic(UserModel user)
        {
            var info = new LogisticInfo();
            foreach(var token in user.UserData.SelectedWBAPITokens)
            {
                var realizations = db.DetailByPeriodModels
                      .Where(o => o.APIKey == token.APIKey 
                      && o.RrDt >= user.UserData.SelectedPeriodFrom
                      && o.RrDt <= user.UserData.SelectedPeriodTo)
                      .ToList();

                info.FromClient += (double)realizations.Where(o => o.DeliveryAmount > 0 && o.DeliveryRub > 0).Sum(o => o.DeliveryRub);
                info.ToClient += (double)realizations.Where(o => o.ReturnAmount > 0 && o.DeliveryRub > 0).Sum(o => o.DeliveryRub);
            }
            return info;
        }
    }
}
