using MarketAI.API.Models;
using MarketWB.Web.Helpers;
using MarketWB.Web.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WBKeysAPI = MarketAI.API.Controllers.WBAPIKeysController;

namespace MarketWB.Web.Controllers.Cabinet
{
    public class WBKeysController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly WBKeysAPI _wbKeysApi;

        public WBKeysController(ILogger<DashboardController> logger, WBKeysAPI wbKeysApi)
        {
            _logger = logger;
            _wbKeysApi = wbKeysApi;
        }

        [HttpPost]
        [Route("WBKeys/CreateWBKey")]
        public async Task<IActionResult> CreateWBKey([FromBody]WBAPITokenModel model)
        {            
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.CreateToken(user.Id, model);
            
            WBParsingJob.ParseImmediatelyIfNewKey(model);
            return new JsonResult("Хуй");
        }
        [HttpDelete]
        [Route("WBKeys/DeleteWBKey")]
        public async Task DeleteWBKey(int id)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.RemoveToken(user.Id, id);
        }



        [HttpPut]
        [Route("WBKeys/SelectWBKey")]
        public async Task<IActionResult> SelectWBKey(int keyId)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetSelectedToken(user.Id, keyId);
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
            await _wbKeysApi.SetChangedPeriodFrom(user.Id, period);
        }
        [HttpPut]
        [Route("WBKeys/SetChangedPeriodTo")]
        public async Task SetChangedPeriodTo(DateTime period)
        {
            var user = await UserHelper.GetUser(User);
            await _wbKeysApi.SetChangedPeriodTo(user.Id, period);
        }

    }
}
