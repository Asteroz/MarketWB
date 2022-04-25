using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class WBAPIKeysModule 
    {
        private readonly ILogger<WBAPIKeysModule> _logger;
        private readonly APIDBContext db;
        public WBAPIKeysModule(ILogger<WBAPIKeysModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public IEnumerable<WBAPITokenModel> GetTokens(int userId)
        {
            return db.WBAPITokens.Include(o => o.Owner).Where(o => o.Owner.Id == userId).ToList();
        }
        public async Task<RequestStatus> RemoveToken(int userId, int id)
        {
            var foundUser = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
            var apiKey = db.WBAPITokens.Include(o => o.Owner).First(o => o.Id == id);


            foundUser.WBAPIKeys.Remove(apiKey);
            db.WBAPITokens.Remove(apiKey);
            db.Users.Update(foundUser);

            await db.SaveChangesAsync();
            return new RequestStatus("Токен успешно удален");
        }
        public async Task<RequestStatus> CreateToken(int userId, WBAPITokenModel token)
        {
            try
            {
                var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                token.Owner = foundUser;

                foundUser.WBAPIKeys.Add(token);
                await db.SaveChangesAsync();
                return new RequestStatus("Токен успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        public async Task<RequestStatus> UpdateToken(WBAPITokenModel updatedToken)
        {
            try
            {
                var foundToken = db.WBAPITokens.FirstOrDefault(o => o.Id == updatedToken.Id);

                if (!db.WBIncomes.Any(o => o.APIKey == updatedToken.APIKey))
                {
                    updatedToken.CreatedFirstTime = true;
                    foundToken.CreatedFirstTime = true;
                }
                

                foundToken.Name = updatedToken.Name;
                foundToken.APIKey = updatedToken.APIKey;

                db.WBAPITokens.Update(foundToken);
                await db.SaveChangesAsync();
                return new RequestStatus("Токен успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }




        public async Task SetSelectedToken(int keyId,bool selected)
        {
            //var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);

            var key = db.WBAPITokens.FirstOrDefault(o => o.Id == keyId);
            key.IsSelected = selected;

            db.WBAPITokens.Update(key);
            await db.SaveChangesAsync();
        }
        public async Task SetSelectedBrand(int userId, string brand)
        {
            var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
            user.UserData.SelectedWBBrand = brand;
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
        public async Task SetSelectedCategory(int userId, string category)
        {
            try
            {
                var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                user.UserData.SelectedWBCategory = category;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task SetChangedPeriodFrom(int userId, DateTime date)
        {
            var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
            user.UserData.SelectedPeriodFrom = date;
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
        public async Task SetChangedPeriodTo(int userId, DateTime date)
        {
            var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
            user.UserData.SelectedPeriodTo = date;
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

    }
}
