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

        public WBKeysController(ILogger<DashboardController> logger, 
                                WBKeysAPI wbKeysApi)
        {
            _logger = logger;
            _wbKeysApi = wbKeysApi;
        }

        [HttpPut]
        [Route("WBKeys/CreateNewWBKey")]
        public async Task<IActionResult> CreateWBKey()
        {
            var user = await UserHelper.GetUser(User);
            var key = new WBAPITokenModel();
            await _wbKeysApi.CreateToken(user.Id, key);
            return new JsonResult(key.Id);
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
            WBParsing.ParseImmediatelyIfNewKey(model);
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
        public async Task SetChangedPeriodFrom(DateTime period)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, period.Date);
        }
        [HttpPut]
        [Route("WBKeys/SetChangedPeriodTo")]
        public async Task SetChangedPeriodTo(DateTime period)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, period.Date.AddHours(23).AddMinutes(59));
        }


        [HttpPut]
        [Route("WBKeys/SetWeekPeriod")]
        public async Task SetWeekPeriod()
        {
            var user = await UserHelper.GetUser(User);

            var firstDayInWeek = DateTime.Now;
            while (firstDayInWeek.DayOfWeek != DayOfWeek.Monday)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            await _wbKeysApi.SetChangedPeriodTo(user.Id, DateTime.Now.Date.AddHours(23).AddMinutes(59));
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, firstDayInWeek);
        }
        [HttpPut]
        [Route("WBKeys/SetMonthPeriod")]
        public async Task SetMonthPeriod()
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, DateTime.Now.Date.AddHours(23).AddMinutes(59));
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, DateTime.Now.AddDays(-DateTime.Now.Day+1));

        }
    }
}
