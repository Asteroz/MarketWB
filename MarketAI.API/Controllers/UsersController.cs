using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<UserModel> GetUsersByPage(int page)
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.Users.Skip(page * 20).ToList();
            }
        }
        [HttpPost]
        public async Task<RequestStatus> CreateUser(UserModel user)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateUser(int id,UserModel user)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var foundUser = db.Users.FirstOrDefault(o => o.Id == id);
                    if(foundUser is null)
                        return new RequestStatus("Пользователя с таким id не существует", 404);

                    user.Id = id;
                    foundUser = user;

                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return new RequestStatus("Пользователь успешно добавлен");
                }
              
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
    }
}
