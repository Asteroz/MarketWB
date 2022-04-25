using MarketAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Promocodes = MarketAI.API.Controllers.PromocodeModule;

namespace MarketWB.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class PromocodesController : Controller
    {
        private readonly ILogger<PromocodesController> _logger;
        private readonly Promocodes _api;
        public PromocodesController(ILogger<PromocodesController> logger, Promocodes api)
        {
            _logger = logger;
            _api = api;
        }
        [Route("Admin/Promocodes")]
        public IActionResult Promocodes()
        {
            var promocodes = _api.GetPromocodes();
            return View("Views/Admin/Promocodes/Promocodes.cshtml", promocodes);
        }
        [Route("Admin/CreatePromocode")]
        public IActionResult CreatePromocode()
        {
            return View("Views/Admin/Promocodes/CreatePromocode.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreatePromocodePOST(PromocodeModel model)
        {
            await _api.CreatePromocode(model);
            return Promocodes();
        }


        [Route("Admin/UpdatePromocode")]
        public IActionResult UpdatePromocode(int id)
        {
            var model = _api.GetPromocode(id);
            return View("Views/Admin/Promocodes/UpdatePromocode.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePromocodePOST(int id,PromocodeModel promocode)
        {
            await _api.UpdatePromocode(id, promocode);
            return Promocodes();
        }


        [Route("Admin/DeletePromocode")]
        public async Task<IActionResult> DeletePromocode(int id)
        {
            await _api.RemovePromocode(id);
            return Promocodes();
        }
    }
}
