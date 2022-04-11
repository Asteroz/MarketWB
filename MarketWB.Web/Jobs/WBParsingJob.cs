using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.WB;
using MarketWB.Parsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WildberriesAPI;
using WildberriesAPI.Models;

namespace MarketWB.Web.Jobs
{

    public static class WBParsingJob
    {
        private static Timer _timer = null;
        static WBParsingJob()
        {
       
        }

        public static async Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(150));
        }

        private static async void DoWork(object state)
        {
            var db = MarketWBParser.db;

            var tokens = await db.WBAPITokens.ToListAsync();
            await Task.Delay(3000);
            foreach (var token in tokens)
            {
                await ParseWBIncomes(token, db);
            }
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await ParseWBOrders(token, db);
            }
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await ParseWBSales(token, db);
            }
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await ParseWBStocks(token, db);
            }
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await ParseDetailByPeriodModels(token, db);
            }
            foreach (var token in tokens)
            {
                await ParseBrandAndCategoryTitles(token, db);
            }
        }

        public static async Task ParseImmediatelyIfNewKey(WBAPITokenModel token)
        {
            var db = MarketWBParser.db;

            token.CreatedFirstTime = true;
            db.WBAPITokens.Update(token);
            await db.SaveChangesAsync();

            await ParseWBIncomes(token, db);
            await Task.Delay(60000);
            await ParseWBOrders(token, db);
            await Task.Delay(60000);
            await ParseWBSales(token, db);
            await Task.Delay(60000);
            await ParseWBStocks(token, db);
            await Task.Delay(60000);
            await ParseDetailByPeriodModels(token, db);
            await ParseBrandAndCategoryTitles(token, db);
        }


        #region Парсинг
        private static async Task ParseWBIncomes(WBAPITokenModel token, APIDBContext db)
        {
            DateTime dateMax = DateTime.Now.AddYears(-3);

            List<WBIncomeModel> data = new List<WBIncomeModel>();
            data = await db.WBIncomes.Where(o => o.APIKey == token.APIKey).ToListAsync();
            if (data.Count > 0)
            {
                dateMax = data.Max(o => o.Date);
                data.Clear();
            }

            var wb = new Wildberries(token.APIKey);

            var parsed = await wb.GetIncomes(dateMax);
            parsed = parsed.Where(o => o.Date > dateMax.AddMinutes(5)).ToList();
            parsed.ForEach(o => o.APIKey = token.APIKey);

            await db.WBIncomes.AddRangeAsync(parsed);
            await db.SaveChangesAsync();
        }
        private static async Task ParseWBOrders(WBAPITokenModel token, APIDBContext db)
        {
            DateTime dateMax = DateTime.Now.AddYears(-3);

            List<WBOrderModel> data = new List<WBOrderModel>();
             data = await db.WBOrders.Where(o => o.APIKey == token.APIKey).ToListAsync();
            if (data.Count > 0)
            {
                dateMax = data.Max(o => o.Date);
                data.Clear();
            }

            var wb = new Wildberries(token.APIKey);

            var parsed = await wb.GetOrders(dateMax);
            parsed = parsed.Where(o => o.Date > dateMax.AddMinutes(5)).ToList();
            parsed.ForEach(o => o.APIKey = token.APIKey);

            await db.WBOrders.AddRangeAsync(parsed);
            await db.SaveChangesAsync();
        }
        private static async Task ParseWBSales(WBAPITokenModel token, APIDBContext db)
        {
            DateTime dateMax = DateTime.Now.AddYears(-3);

            List<WBSaleModel> data = new List<WBSaleModel>();
            data = await db.WBSales.Where(o => o.APIKey == token.APIKey).ToListAsync();
            if (data.Count > 0)
            {
                dateMax = data.Max(o => o.Date);
                data.Clear();
            }

            var wb = new Wildberries(token.APIKey);

            var parsed = await wb.GetSales(dateMax);
            parsed = parsed.Where(o => o.Date > dateMax.AddMinutes(5)).ToList();
            parsed.ForEach(o => o.APIKey = token.APIKey);

            await db.WBSales.AddRangeAsync(parsed);
            await db.SaveChangesAsync();
        }
        private static async Task ParseWBStocks(WBAPITokenModel token, APIDBContext db)
        {
            DateTime dateMax = DateTime.Now.AddYears(-3);

            List<WBStockModel> data = new List<WBStockModel>();
            data = await db.WBStocks.Where(o => o.APIKey == token.APIKey).ToListAsync();
            if (data.Count > 0)
            {
                dateMax = data.Max(o => o.LastChangeDate);
                data.Clear();
            }

            var wb = new Wildberries(token.APIKey);

            var parsed = await wb.GetStocks(dateMax);
            parsed = parsed.Where(o => o.LastChangeDate > dateMax.AddMinutes(5)).ToList();
            parsed.ForEach(o => o.APIKey = token.APIKey);

            await db.WBStocks.AddRangeAsync(parsed);
            await db.SaveChangesAsync();
        }
        private static async Task ParseDetailByPeriodModels(WBAPITokenModel token, APIDBContext db)
        {
            DateTime dateMax = DateTime.Now.AddYears(-3);

            List<DetailByPeriodModel> data = new List<DetailByPeriodModel>();
            data = await db.DetailByPeriodModels.Where(o => o.APIKey == token.APIKey).ToListAsync();
            if (data.Count > 0)
            {
                dateMax = data.Max(o => o.RrDt);
                data.Clear();
            }

            var wb = new Wildberries(token.APIKey);

            var parsed = await wb.GetReportDetailByPeriod(dateMax,DateTime.Now);
            parsed = parsed.Where(o => o.RrDt > dateMax.AddMinutes(5)).ToList();
            parsed.ForEach(o => o.APIKey = token.APIKey);

            await db.DetailByPeriodModels.AddRangeAsync(parsed);
            await db.SaveChangesAsync();
        }

        private static async Task ParseBrandAndCategoryTitles(WBAPITokenModel token, APIDBContext db)
        {
            var data = await db.WBSales.Where(o => o.APIKey == token.APIKey).ToListAsync();
            SortedSet<string> brands = new SortedSet<string>();
            SortedSet<string> categories = new SortedSet<string>();
            foreach (var item in data)
            {
                brands.Add(item.Brand);
                categories.Add(item.Subject);
            }
            foreach (var item in brands)
            {
                var foundBrand = db.AvailableWBBrands
                    .FirstOrDefault(o => o.Brand == item
                                    && o.APIKey == token.APIKey);
                if (foundBrand is null)
                {
                    db.AvailableWBBrands.Add(new AvailableWBBrand
                    {
                        APIKey = token.APIKey,
                        Brand = item
                    });
                }
            }
            foreach (var item in categories)
            {
                var foundCategory = db.AvailableWBCategories
                    .FirstOrDefault(o => o.Category == item
                                    && o.APIKey == token.APIKey);
                if (foundCategory is null)
                {
                    db.AvailableWBCategories.Add(new AvailableWBCategory
                    {
                        APIKey = token.APIKey,
                        Category = item
                    });
                }
            }
            await db.SaveChangesAsync();
        }
        #endregion

        public static Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public static void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
