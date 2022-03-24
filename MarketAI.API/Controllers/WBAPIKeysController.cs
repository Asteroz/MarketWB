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
        public async Task<RequestStatus> RemovePromocode(int userId, int id)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                if (foundUser is null)
                    return new RequestStatus("Пользователя с таким id не существует", 404);


                var apiKey = db.WBAPITokens.Include(o => o.Owner).First(o => o.Id == id);
                if (apiKey == null)
                    return new RequestStatus("Токена с таким id не существует", 404);
                if(apiKey.Owner.Id != foundUser.Id)
                    return new RequestStatus("Пользователь не владеет данным ключом", 400);

                db.WBAPITokens.Remove(apiKey);
                await db.SaveChangesAsync();
            }
            return new RequestStatus("Токен успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreatePromocode(int userId, WBAPITokenModel token)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == userId);
                    if (foundUser is null)
                        return new RequestStatus("Пользователя с таким id не существует", 404);

                    if(foundUser.WBAPIKeys.Any(o => o.Name == token.Name))
                        return new RequestStatus("Ключ с таким названием уже существует", 400);
                    if (foundUser.WBAPIKeys.Any(o => o.APIKey == token.APIKey))
                        return new RequestStatus("Ключ с таким api токеном уже существует", 400);

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
        public async Task<RequestStatus> UpdatePromocode(int userId, WBAPITokenModel updatedToken)
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
    }
}
