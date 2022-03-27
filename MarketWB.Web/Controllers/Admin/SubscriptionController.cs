using MarketAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subscriptions = MarketAI.API.Controllers.SubscriptionsController;

namespace MarketWB.Web.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly Subscriptions _api;
        public SubscriptionController(ILogger<SubscriptionController> logger, Subscriptions api)
        {
            _logger = logger;
            _api = api;
        }
        [Route("Admin/Subscriptions")]
        public IActionResult Subscriptions()
        {
            var subscriptions = _api.GetSubscriptions();
            return View("Views/Admin/Subscriptions/Subscriptions.cshtml", subscriptions);
        }
        [Route("Admin/CreateSubscription")]
        public IActionResult CreateSubscription()
        {
            return View("Views/Admin/Subscriptions/CreateSubscription.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubscriptionPOST(SubscriptionModel model)
        {
            await _api.CreateSubscription(model);
            return Subscriptions();
        }


        [Route("Admin/UpdateSubscription")]
        public IActionResult UpdateSubscription(SubscriptionModel model)
        {
            return View("Views/Admin/Subscriptions/UpdateSubscription.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubscriptionPOST(SubscriptionModel model)
        {
            await _api.UpdateSubscription(model);
            return Subscriptions();
        }


        [Route("Admin/DeleteSubscription")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            await _api.RemoveSubscription(id);
            return Subscriptions();
        }
    }
}
