using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Admin
{
    public class SubscriptionController : Controller
    {
        private readonly ILogger<SubscriptionController> _logger;
        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;
        }
        [Route("Admin/Subscriptions")]
        public IActionResult Subscriptions()
        {
            return View("Views/Admin/Subscriptions/Subscriptions.cshtml");
        }
        [Route("Admin/CreateSubscription")]
        public IActionResult CreateSubscription()
        {
            return View("Views/Admin/Promocodes/CreateSubscription.cshtml");
        }
        [Route("Admin/UpdateSubscription")]
        public IActionResult UpdateSubscription()
        {
            return View("Views/Admin/Promocodes/UpdateSubscription.cshtml");
        }
    }
}
