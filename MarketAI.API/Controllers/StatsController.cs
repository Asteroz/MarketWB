using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly APIDBContext db;
        public StatsController(APIDBContext _db)
        {
            db = _db;
        }

        [HttpGet]
        public async Task AddAuthStats(UserModel user)
        {
            var entry = new Models.Stats.AuthStatsModel
            {
                Date = DateTime.Now,
            };
            user.Auths.Add(entry);
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
        [HttpGet]
        public List<AuthStatsModel> GetVisitors()
        {
            return db.AuthStatsModels.Include(o => o.User).ToList();
        }


        [HttpGet]
        public void SetUserOnline(UserModel user,bool isOnline)
        {
            user.IsOnline = isOnline;
            db.Users.Update(user);
            db.SaveChanges();
        }
        [HttpGet]
        public List<UserModel> GetOnlineUsers()
        {
            return db.Users.Where(o => o.IsOnline).ToList();
        }




        [HttpGet]
        public List<PaymentModel> GetUserPayments()
        {
            return db.PaymentModels.Include(o => o.User).ToList();
        }

        [HttpPut]
        public void RegisterUserPayment(PaymentModel payment)
        {
            db.PaymentModels.Add(payment);
            db.SaveChanges();
        }

    }
}
