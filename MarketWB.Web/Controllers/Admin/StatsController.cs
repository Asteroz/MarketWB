using MarketAI.API.Core;
using MarketWB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stats = MarketAI.API.Controllers.StatsModule;

namespace MarketWB.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class StatsController : Controller
    {
        private readonly ILogger<StatsController> _logger;
        private readonly Stats _stats;
        private readonly APIDBContext _db;
        public StatsController(ILogger<StatsController> logger, Stats stats, APIDBContext db)
        {
            _logger = logger;
            _stats = stats;
            _db = db;
        }

        [Route("Admin/Visitors")]
        public IActionResult Visitors()
        {
            List<GroupedVisitorsByDay> stats = new List<GroupedVisitorsByDay>();

            var visitors = _stats.GetVisitors().GroupBy(o => o.Date.Date);
            foreach (var day in visitors)
            {
                stats.Add(new GroupedVisitorsByDay
                {
                    Date = day.Key,
                    Visitors = day.Count()
                });
            }

            return View("Views/Admin/Stats/Visitors.cshtml", stats);
        }
        [Route("Admin/Payments")]
        public IActionResult Payments()
        {
            var payments = _stats.GetUserPayments();
            return View("Views/Admin/Stats/Payments.cshtml", payments);
        }
        [Route("Admin/Online")]
        public IActionResult Online()
        {
            var online = _stats.GetOnlineUsers();
            return View("Views/Admin/Stats/Online.cshtml", online);
        }
        [Route("Admin/WithdrawRequests")]
        public IActionResult WithdrawRequests()
        {
            var requests = _db.WithdrawRequests.Include(o => o.User).ToList();
            requests.Reverse();
            return View("Views/Admin/Stats/WithdrawRequests.cshtml", requests);
        }
    }
}
