using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Stats;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class StatsModule 
    {
        private readonly APIDBContext db;
        public StatsModule(APIDBContext _db)
        {
            db = _db;
        }

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

        public List<AuthStatsModel> GetVisitors()
        {
            return db.AuthStatsModels.Include(o => o.User).ToList();
        }


        public void SetUserOnline(UserModel user,bool isOnline)
        {
            user.IsOnline = isOnline;
            db.Users.Update(user);
            db.SaveChanges();
        }
        public List<UserModel> GetOnlineUsers()
        {
            return db.Users.Where(o => o.IsOnline).ToList();
        }




        public List<PaymentModel> GetUserPayments()
        {
            return db.PaymentModels.Include(o => o.User).ToList();
        }

        public void RegisterUserPayment(PaymentModel payment)
        {
            db.PaymentModels.Add(payment);
            db.SaveChanges();
        }

    }
}
