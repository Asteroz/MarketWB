// See https://aka.ms/new-console-template for more information
using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketWB.Web.Jobs;

APIDBContext _db = new APIDBContext();

Console.WriteLine("Hello, World!");
Timer timer = new Timer(DoWork,null,TimeSpan.Zero,TimeSpan.FromMinutes(60));

while (true)
    Console.ReadLine();

async void DoWork(object state)
{
    List<WBAPITokenModel> tokens = new List<WBAPITokenModel>();





    try
    {
        tokens = _db.WBAPITokens.ToList();
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }


    await Task.Delay(3000);
    Console.WriteLine("ParseWBIncomes");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseWBIncomes(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }

    await Task.Delay(60000);
    Console.WriteLine("ParseWBOrders");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseWBOrders(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }


    await Task.Delay(60000);
    Console.WriteLine("ParseWBSales");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseWBSales(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }
    await Task.Delay(60000);
    Console.WriteLine("ParseWBStocks");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseWBStocks(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }

    await Task.Delay(60000);
    Console.WriteLine("ParseDetailByPeriodModels");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseDetailByPeriodModels(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }
    await Task.Delay(60000);
    Console.WriteLine("ParseBrandAndCategoryTitles");
    foreach (var token in tokens)
    {
        try
        {
            await WBParsing.ParseBrandAndCategoryTitles(token, _db);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); }
    }

    Console.WriteLine("Парсинг выполнен. Ждем 60 минут");
    await Task.Delay(TimeSpan.FromMinutes(60));
}
