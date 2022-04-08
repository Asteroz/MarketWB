using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WildberriesAPI.Helpers;
using WildberriesAPI.Models;

namespace WildberriesAPI
{
    public class Wildberries
    {
        private const string HOST = "https://suppliers-stats.wildberries.ru/api/v1";
        private string _apikey;

        public Wildberries(string apikey)
        {
            _apikey = apikey;
        }
        
        public async Task<List<string>> GetOurBrands()
        {
            SortedSet<string> brands = new SortedSet<string>();

            var stocks = await GetStocks(DateTime.Now.AddYears(-4));
            foreach(var stock in stocks)
            {
                if (!string.IsNullOrEmpty(stock.Brand))
                {
                    brands.Add(stock.Brand);
                }
            }

            return brands.ToList();
        }
        public async Task<List<string>> GetOurSubjects()
        {
            SortedSet<string> brands = new SortedSet<string>();

            var stocks = await GetStocks(DateTime.Now.AddYears(-4));
            foreach (var stock in stocks)
            {
                if (!string.IsNullOrEmpty(stock.Subject))
                {
                    brands.Add(stock.Subject);
                }
            }
            return brands.ToList();
        }



        public async Task<List<WBSaleModel>> GetSales(DateTime from)
        {
            string url = HOST + $"/supplier/sales?" +
                $"dateFrom={from.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}" +
                $"&key={_apikey}";

            var response = await RequestHelper.MakeRequest(url);
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            List<WBSaleModel> sales = new List<WBSaleModel>();
            foreach (var item in data)
            {
                try
                {
                    sales.Add(JsonConvert.DeserializeObject<WBSaleModel>(item.ToString()));
                }
                catch (Exception ex) { }         
            }       
            return sales;
        }
        public async Task<List<DetailByPeriodModel>> GetReportDetailByPeriod(DateTime from, DateTime to)
        {
            string url = HOST + $"/supplier/reportDetailByPeriod?" +
                $"dateFrom={from.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}" +
                $"&key={_apikey}" +
                $"&limit=1000000" +
                $"&rrdid=0" +
                $"&dateto={to.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}";

            var response = await RequestHelper.MakeRequest(url);
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            List<DetailByPeriodModel> sales = new List<DetailByPeriodModel>();
            foreach (var item in data)
            {
                try
                {
                    sales.Add(JsonConvert.DeserializeObject<DetailByPeriodModel>(item.ToString()));
                }
                catch (Exception ex) { }
            }
            return sales;
        }
        public async Task<List<WBOrderModel>> GetOrders(DateTime from)
        {
            string url = HOST + $"/supplier/orders?" +
                $"dateFrom={from.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}" +
                $"&key={_apikey}";

            var response = await RequestHelper.MakeRequest(url);
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            List<WBOrderModel> sales = new List<WBOrderModel>();
            foreach (var item in data)
            {
                try
                {
                    sales.Add(JsonConvert.DeserializeObject<WBOrderModel>(item.ToString()));
                }
                catch (Exception ex) {
                }
            }
            return sales;
        }
        public async Task<List<WBStockModel>> GetStocks(DateTime from)
        {
            string url = HOST + $"/supplier/stocks?" +
                $"dateFrom={from.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}" +
                $"&key={_apikey}";

            var response = await RequestHelper.MakeRequest(url);
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            List<WBStockModel> sales = new List<WBStockModel>();
            foreach (var item in data)
            {
                try
                {
                    sales.Add(JsonConvert.DeserializeObject<WBStockModel>(item.ToString()));
                }
                catch (Exception ex)
                {
                }
            }
            return sales;
        }
        public async Task<List<WBIncomeModel>> GetIncomes(DateTime from)
        {
            string url = HOST + $"/supplier/incomes?" +
                $"dateFrom={from.ToString("yyyy-MM-ddThh:mm:ss.FFFZ")}" +
                $"&key={_apikey}";

            var response = await RequestHelper.MakeRequest(url);
            dynamic data = JsonConvert.DeserializeObject(response.Content);

            List<WBIncomeModel> sales = new List<WBIncomeModel>();
            foreach (var item in data)
            {
                try
                {
                    sales.Add(JsonConvert.DeserializeObject<WBIncomeModel>(item.ToString()));
                }
                catch (Exception ex) { }
            }
            return sales;
        }

    }
}
