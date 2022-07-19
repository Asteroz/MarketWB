using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketWB.Web.Helpers;
using MarketWB.Web.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WBKeysAPI = MarketAI.API.Controllers.WBAPIKeysModule;

namespace MarketWB.Web.Controllers.Cabinet
{
    public class WBKeysController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly WBKeysAPI _wbKeysApi;
        private readonly APIDBContext _db;
        public WBKeysController(ILogger<DashboardController> logger, 
                                WBKeysAPI wbKeysApi,
                                APIDBContext db)
        {
            _logger = logger;
            _wbKeysApi = wbKeysApi;
            _db = db;
        }

        [HttpPut]
        [Route("WBKeys/CreateNewWBKey")]
        public async Task<IActionResult> CreateNewWBKey()
        {
            var user = await UserHelper.GetUser(User);
            var key = new WBAPITokenModel();
            await _wbKeysApi.CreateToken(user.Id, key);
            return new JsonResult(key.Id);
        }
        [HttpPost]
        [Route("WBKeys/CreateWBKey")]
        public async Task<IActionResult> CreateWBKey( [FromForm]WBAPITokenModel model)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.CreateToken(user.Id, model);
            WBParsing.ParseImmediatelyIfNewKey(model, _db);
            return RedirectToAction("ApiKeys", "Cabinet");
        }



        [HttpDelete]
        [Route("WBKeys/DeleteWBKey")]
        public async Task DeleteWBKey(int id)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.RemoveToken(user.Id, id);
        }
        [HttpPost]
        [Route("WBKeys/UpdateWBKey")]
        public async Task UpdateWBKey([FromBody] WBAPITokenModel model)
        {
            await _wbKeysApi.UpdateToken(model);
            WBParsing.ParseImmediatelyIfNewKey(model, _db);
        }








        [HttpPut]
        [Route("WBKeys/SelectWBKey")]
        public async Task<IActionResult> SelectWBKey(int keyId,bool isChecked)
        {
            await _wbKeysApi.SetSelectedToken(keyId, isChecked);
            return new JsonResult("Хуй");
        }
        [HttpPut]
        [Route("WBKeys/SelectWBCategory")]
        public async Task<IActionResult> SelectWBCategory(string category)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetSelectedCategory(user.Id, category);
            return new JsonResult("Хуй");
        }
        [HttpPut]
        [Route("WBKeys/SelectWBBrand")]
        public async Task<IActionResult> SelectWBBrand(string brand)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetSelectedBrand(user.Id, brand);
            return new JsonResult("Хуй");
        }
        [HttpPut]
        [Route("WBKeys/SetChangedPeriodFrom")]
        public async Task SetChangedPeriodFrom(string period)
        {
            DateTime date = DateTime.Parse(period);
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, date.Date);
        }
        [HttpPut]
        [Route("WBKeys/SetChangedPeriodTo")]
        public async Task SetChangedPeriodTo(string period)
        {
            DateTime date = DateTime.Parse(period);
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, date.Date.AddHours(23).AddMinutes(59));
        }



        [HttpPut]
        [Route("WBKeys/SetYesterdayPeriod")]
        public async Task SetYesterdayPeriod()
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, DateTime.Now.AddHours(12).Date.AddDays(-1));
            await _wbKeysApi.SetChangedPeriodTo(user.Id, DateTime.Now.AddHours(12).Date.AddDays(-1).AddHours(23).AddMinutes(59));
        }


        [HttpPut]
        [Route("WBKeys/SetWeekPeriod")]
        public async Task SetWeekPeriod()
        {
            var user = await UserHelper.GetUser(User);

            var firstDayInWeek = DateTime.Now;
            while (firstDayInWeek.DayOfWeek != DayOfWeek.Monday)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            await _wbKeysApi.SetChangedPeriodFrom(user.Id, firstDayInWeek);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, DateTime.Now.AddHours(12).Date.AddHours(23).AddMinutes(59));
        }
        [HttpPut]
        [Route("WBKeys/SetMonthPeriod")]
        public async Task SetMonthPeriod()
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, DateTime.Now.AddHours(12).Date.AddHours(23).AddMinutes(59));
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, DateTime.Now.AddHours(12).AddDays(-DateTime.Now.Day+1));
        }
    }
}
