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

    public class WBParsingJob : IHostedService, IDisposable
    {
        private Timer _timer = null;
        private APIDBContext _db;
        public WBParsingJob()
        {
            _db = new APIDBContext();
            WBParsing._db = _db;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(150));
        }

        private async void DoWork(object state)
        {
            var tokens = _db.WBAPITokens.ToList();
            await Task.Delay(3000);
            foreach (var token in tokens)
            {
                await WBParsing.ParseWBIncomes(token);
            }
            _db = new APIDBContext();
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await WBParsing.ParseWBOrders(token);
            }
            _db = new APIDBContext();
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await WBParsing.ParseWBSales(token);
            }
            _db = new APIDBContext();
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await WBParsing.ParseWBStocks(token);
            }
            _db = new APIDBContext();
            await Task.Delay(60000);
            foreach (var token in tokens)
            {
                await WBParsing.ParseDetailByPeriodModels(token);
            }
            foreach (var token in tokens)
            {
                await WBParsing.ParseBrandAndCategoryTitles(token);
            }

        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public static class WBParsing
    {
        public static APIDBContext _db;

        #region Парсинг
        public static async Task ParseWBIncomes(WBAPITokenModel token)
        {
            var wb = new Wildberries(token.APIKey);

            var lastDate = DateTime.Now.AddYears(-4);
            if(_db.WBIncomes.Where(o => o.APIKey == token.APIKey).Any())
                lastDate = _db.WBIncomes.Where(o => o.APIKey == token.APIKey).Max(o => o.Date);

            var parsed = await wb.GetIncomes(lastDate.AddMinutes(5));
            parsed.ForEach(o => o.APIKey = token.APIKey);


            _db.WBIncomes.AddRange(parsed);
            _db.SaveChanges();


            GC.Collect();

        }
        public static async Task ParseWBOrders(WBAPITokenModel token)
        {
            var wb = new Wildberries(token.APIKey);

            var lastDate = DateTime.Now.AddYears(-4);
            if (_db.WBOrders.Where(o => o.APIKey == token.APIKey).Any())
                lastDate = _db.WBOrders.Where(o => o.APIKey == token.APIKey).Max(o => o.Date);

            var parsed = await wb.GetOrders(lastDate.AddMinutes(5));
            parsed.ForEach(o => o.APIKey = token.APIKey);

            _db.WBOrders.AddRange(parsed);
            _db.SaveChanges();

            GC.Collect();
        }
        public static async Task ParseWBSales(WBAPITokenModel token)
        {
            var wb = new Wildberries(token.APIKey);

            var lastDate = DateTime.Now.AddYears(-4);
            if (_db.WBSales.Where(o => o.APIKey == token.APIKey).Any())
                lastDate = _db.WBSales.Where(o => o.APIKey == token.APIKey).Max(o => o.Date);

            var parsed = await wb.GetSales(lastDate.AddMinutes(5));
            parsed.ForEach(o => o.APIKey = token.APIKey);

            _db.WBSales.AddRange(parsed);
            _db.SaveChanges();

            GC.Collect();
        }
        public static async Task ParseWBStocks(WBAPITokenModel token)
        {
            var wb = new Wildberries(token.APIKey);

            var lastDate = DateTime.Now.AddYears(-4);

            var parsed = await wb.GetStocks(lastDate.AddMinutes(5));
            parsed.ForEach(o => o.APIKey = token.APIKey);

            _db.WBStocks.RemoveRange(_db.WBStocks.Where(o => o.APIKey == token.APIKey));
            _db.WBStocks.AddRange(parsed);
            _db.SaveChanges();

            GC.Collect();
        }
        public static async Task ParseDetailByPeriodModels(WBAPITokenModel token)
        {       
            var wb = new Wildberries(token.APIKey);

            var lastDate = DateTime.Now.AddYears(-4);
            if (_db.DetailByPeriodModels.Where(o => o.APIKey == token.APIKey).Any())
                lastDate = _db.DetailByPeriodModels.Where(o => o.APIKey == token.APIKey).Max(o => o.RrDt);

            var parsed = await wb.GetReportDetailByPeriod(lastDate.AddMinutes(5), DateTime.Now);
            parsed.ForEach(o => o.APIKey = token.APIKey);

            _db.DetailByPeriodModels.AddRange(parsed);
            _db.SaveChanges();
        }

        public static async Task ParseBrandAndCategoryTitles(WBAPITokenModel token)
        {
            var data = _db.WBSales.Where(o => o.APIKey == token.APIKey).ToList();
            SortedSet<string> brands = new SortedSet<string>();
            SortedSet<string> categories = new SortedSet<string>();
            foreach (var item in data)
            {
                brands.Add(item.Brand);
                categories.Add(item.Subject);
            }
            foreach (var item in brands)
            {
                var foundBrand = _db.AvailableWBBrands
                    .FirstOrDefault(o => o.Brand == item
                                    && o.APIKey == token.APIKey);
                if (foundBrand is null)
                {
                    _db.AvailableWBBrands.Add(new AvailableWBBrand
                    {
                        APIKey = token.APIKey,
                        Brand = item
                    });
                }
            }
            foreach (var item in categories)
            {
                var foundCategory = _db.AvailableWBCategories
                    .FirstOrDefault(o => o.Category == item
                                    && o.APIKey == token.APIKey);
                if (foundCategory is null)
                {
                    _db.AvailableWBCategories.Add(new AvailableWBCategory
                    {
                        APIKey = token.APIKey,
                        Category = item
                    });
                }
            }
            _db.SaveChanges();
        }

        #endregion



        public static async Task ParseImmediatelyIfNewKey(WBAPITokenModel token)
        {
            try
            {
                if (!token.CreatedFirstTime) return;

                await ParseWBIncomes(token);
                await Task.Delay(60000);
                await ParseWBOrders(token);
                await Task.Delay(60000);
                await ParseWBSales(token);
                await Task.Delay(60000);
                await ParseWBStocks(token);
                await Task.Delay(60000);
                await ParseDetailByPeriodModels(token);
                await ParseBrandAndCategoryTitles(token);
            }
            catch (Exception ex)
            {

            }
           
        }

        public static async Task RefreshArticles(UserModel owner)
        {
            foreach (var token in owner.WBAPIKeys)
            {
                var orders = _db.WBOrders.Where(o => o.APIKey == token.APIKey).ToList();
                foreach (var item in orders)
                {
                    if (!owner.SelfCosts.Any(o => o.ProductId == item.NmId))
                    {
                        _db.SelfCosts.Add(new SelfCostModel { ProductId = (long)item.NmId,OwnerId= owner.Id });
                    }
                }
              //   _db.Users.Update(owner);
                _db.SaveChanges();
            }

        }

        public static async Task SetSelfBuyStatus(long nmId, bool isSelfBuy)
        {
            var orders = _db.WBOrders.Where(o => o.NmId == nmId).ToList();
            var sales = _db.WBSales.Where(o => o.NmId == nmId).ToList();

            orders.ForEach(o => o.IsSelfBuy = isSelfBuy);
            sales.ForEach(o => o.IsSelfBuy = isSelfBuy);

            _db.WBOrders.UpdateRange(orders);
            _db.WBSales.UpdateRange(sales);

            _db.SaveChanges();
        }
    }
}
