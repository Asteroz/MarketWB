using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WBAPIKeysController : ControllerBase
    {
        private readonly ILogger<WBAPIKeysController> _logger;
        public WBAPIKeysController(ILogger<WBAPIKeysController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WBAPITokenModel> GetTokens(int userId)
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.WBAPITokens.Include(o => o.Owner).Where(o => o.Owner.Id == userId).ToList();
            }
        }
        [HttpDelete]
        public async Task<RequestStatus> RemoveToken(int userId, int id)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var foundUser = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                var apiKey = db.WBAPITokens.Include(o => o.Owner).First(o => o.Id == id);

                if(foundUser.UserData.SelectedWBAPITokenId == id)
                {
                    foundUser.UserData.SelectedWBAPIToken = null;
                }

                foundUser.WBAPIKeys.Remove(apiKey);
                db.WBAPITokens.Remove(apiKey);
                db.Users.Update(foundUser);

                await db.SaveChangesAsync();
            }
            return new RequestStatus("Токен успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreateToken(int userId, WBAPITokenModel token)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                    token.Owner = foundUser;

                    foundUser.WBAPIKeys.Add(token);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Токен успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateToken(int userId, WBAPITokenModel updatedToken)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                    if (foundUser is null)
                        return new RequestStatus("Пользователя с таким id не существует", 404);

                    var foundToken = foundUser.WBAPIKeys.FirstOrDefault(o => o.Id == updatedToken.Id);
                    if (foundToken is null)
                        return new RequestStatus("Токена с таким id не существует", 404);

                    if (foundToken.Name == updatedToken.Name)
                        return new RequestStatus("Ключ с таким названием уже существует", 400);
                    if (foundToken.APIKey == updatedToken.APIKey)
                        return new RequestStatus("Ключ с таким api токеном уже существует", 400);

                    foundToken = updatedToken;
                    foundToken.Owner = foundUser;

                    db.WBAPITokens.Update(foundToken);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Токен успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }




        public async Task SetSelectedToken(int userId,int keyId)
        {
            using (APIDBContext db = new APIDBContext())
            {
               var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
               user.UserData.SelectedWBAPITokenId = keyId;
               db.Users.Update(user);
               await db.SaveChangesAsync();
            }
        }
        public async Task SetSelectedBrand(int userId, string brand)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                user.UserData.SelectedWBBrand = brand;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }
        public async Task SetSelectedCategory(int userId, string category)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                user.UserData.SelectedWBCategory = category;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }
        public async Task SetChangedPeriodFrom(int userId, DateTime date)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                user.UserData.SelectedPeriodFrom = date;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }
        public async Task SetChangedPeriodTo(int userId, DateTime date)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var user = db.Users.Include(o => o.UserData).FirstOrDefault(o => o.Id == userId);
                user.UserData.SelectedPeriodTo = date;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }

    }
}
