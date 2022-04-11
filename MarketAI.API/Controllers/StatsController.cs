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
        public StatsController()
        {

        }

        [HttpGet]
        public async Task AddAuthStats(UserModel user)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var entry = new Models.Stats.AuthStatsModel
                {
                    Date = DateTime.Now,
                };
                user.Auths.Add(entry);
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }
        [HttpGet]
        public List<AuthStatsModel> GetVisitors()
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.AuthStatsModels.Include(o => o.User).ToList();
            }
        }


        [HttpGet]
        public void SetUserOnline(UserModel user,bool isOnline)
        {
            user.IsOnline = isOnline;
            using (APIDBContext db = new APIDBContext())
            {
                db.Users.Update(user);
                db.SaveChanges();
            }
        }
        [HttpGet]
        public List<UserModel> GetOnlineUsers()
        {
            using (APIDBContext db = new APIDBContext())
            {
              return db.Users.Where(o => o.IsOnline).ToList();
            }
        }




        [HttpGet]
        public List<PaymentModel> GetUserPayments()
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.PaymentModels.Include(o => o.User).ToList();
            }
        }

        [HttpPut]
        public void RegisterUserPayment(PaymentModel payment)
        {
            using (APIDBContext db = new APIDBContext())
            {
                db.PaymentModels.Add(payment);
                db.SaveChanges();
            }
        }

    }
}
