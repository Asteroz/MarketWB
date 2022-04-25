using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmsAero;
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
        private readonly APIDBContext db;
        public UsersController(ILogger<UsersController> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        [HttpGet]
        public async Task<UserModel> GetUserById(int id)
        {
            var found = db.Users
                   .Include(o => o.UserData)
                   .Include(o => o.UserData.SelectedWBAPIToken.SelfCosts)
                   .Include(o => o.UserData.SelectedWBAPIToken.ExtraExpenses)
                   .Include(o => o.WBAPIKeys)
                   .FirstOrDefault(o => o.Id == id);
            return found;
        }
        [HttpGet]
        public UserModel GetUserByCredintials(string login,string password)
        {
            var found = db.Users.AsNoTracking()
                   .Include(o => o.UserData)
                   .Include(o => o.UserData.SelectedWBAPIToken.SelfCosts)
                   .Include(o => o.UserData.SelectedWBAPIToken.ExtraExpenses)
                   .Include(o => o.WBAPIKeys)
                   .FirstOrDefault(o => (o.Phone.Equals(login) || o.Email.Equals(login)) && o.Password == password);
            return found;
        }
        public UserModel GetUserByPhoneOrEmail(string login)
        {
            var found = db.Users.AsNoTracking()
                     .Include(o => o.UserData)
                     .Include(o => o.UserData.SelectedWBAPIToken.SelfCosts)
                     .Include(o => o.UserData.SelectedWBAPIToken.ExtraExpenses)
                     .Include(o => o.WBAPIKeys)
                     .FirstOrDefault(o => (o.Phone.Equals(login) || o.Email.Equals(login)));
            return found;
        }
        [HttpGet]
        public IEnumerable<UserModel> GetUsersByPage(int page)
        {
            return db.Users.AsNoTracking().Include(o => o.UserData).Skip(page * 20).ToList();
        }






        [HttpPost]
        public async Task<RequestStatus> CreateUser(UserModel user)
        {
            try
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();

                user.UserData = new UserData();

                db.Users.Update(user);
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateUser(UserModel user)
        {
            try
            {
                var found = db.Users.FirstOrDefault(o => o.Id == user.Id);
                found.ActivatedPromocode = user.ActivatedPromocode;
                found.Password = user.Password;

                db.Users.Update(found);
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPost]
        public async Task<RequestStatus> ChangePassword([FromBody]UserModel user)
        {
            try
            {
                var found = db.Users.FirstOrDefault(o => o.Phone == user.Phone);
                found.Password = user.Password;

                db.Users.Update(found);
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPost]
        public async Task<RequestStatus> ActivatePromocode(UserModel user)
        {
            try
            {
                var found = db.Users.FirstOrDefault(o => o.Id == user.Id);
                found.ActivatedPromocode = user.ActivatedPromocode;
                db.Users.Update(found);
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }





        public async Task SendSMSCode(string phone)
        {
            string code = new Random().Next(100000, 999999).ToString();
            string text = $"Код подтверждения : {code}";
            string sign = $"SMS Aero";
            await SendSMSCode(phone, text, sign, code);
        }

        public async Task<string> GetSMSCode(string phone)
        {
            return db.SMSCodes.FirstOrDefault(o => o.Phone == phone)?.Code;
        }

        [NonAction]
        public async Task SendSMSCode(string phone, string text, string sign,string code)
        {
            await new SMSAero().SendSMS(phone, text, sign);
            var oldCodes = db.SMSCodes.Where(o => o.Phone == phone);
            db.SMSCodes.RemoveRange(oldCodes);

            db.SMSCodes.Add(new SMSCode { Phone = phone, Code = code });
            await db.SaveChangesAsync();
        }
    }
}
