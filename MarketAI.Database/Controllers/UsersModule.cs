using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmsAero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class UsersModule 
    {
        private readonly ILogger<UsersModule> _logger;
        private readonly APIDBContext db;
        public UsersModule(ILogger<UsersModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        public UserModel GetUserByRefCode(string refCode)
        {
            var found = db.Users
                 .Include(o => o.UserData)
                 .Include(o => o.SelfCosts)
                 .Include(o => o.ExtraExpenses)
                 .Include(o => o.Referrals).ThenInclude(o => o.HisRef).ThenInclude(o => o.Payments)
                 .Include(o => o.UserData.SelectedWBAPITokens)
                 .Include(o => o.WBAPIKeys)
                 .FirstOrDefault(o => o.ReferralCode == refCode);

            if(found != null)
                found.UserData.SelectedWBAPITokens = found.WBAPIKeys.Where(o => o.IsSelected).ToList();
            return found;
        }

        public async Task<UserModel> GetUserById(int id)
        {
            var found = db.Users
                   .Include(o => o.UserData)
                   .Include(o => o.SelfCosts)
                   .Include(o => o.ExtraExpenses)
                   .Include(o => o.Referrals).ThenInclude(o => o.HisRef).ThenInclude(o => o.Payments)
                   .Include(o => o.UserData.SelectedWBAPITokens)
                   .Include(o => o.WBAPIKeys)
                   .FirstOrDefault(o => o.Id == id);
            if (found != null)
                found.UserData.SelectedWBAPITokens = found.WBAPIKeys.Where(o => o.IsSelected).ToList();
            return found;
        }
        public UserModel GetUserByCredintials(string login,string password)
        {
            var found = db.Users.AsNoTracking()
                   .Include(o => o.UserData)
                   .Include(o => o.SelfCosts)
                   .Include(o => o.Referrals).ThenInclude(o => o.HisRef).ThenInclude(o => o.Payments)
                   .Include(o => o.ExtraExpenses)
                   .Include(o => o.UserData.SelectedWBAPITokens)
                   .Include(o => o.WBAPIKeys)
                   .FirstOrDefault(o => (o.Phone.Equals(login) || o.Email.Equals(login)) && o.Password == password);
            if (found != null)
                found.UserData.SelectedWBAPITokens = found.WBAPIKeys.Where(o => o.IsSelected).ToList();
            return found;
        }
        public UserModel GetUserByPhoneOrEmail(string login)
        {
            var found = db.Users.AsNoTracking()
                       .Include(o => o.UserData)
                       .Include(o => o.SelfCosts)
                       .Include(o => o.Referrals).ThenInclude(o => o.HisRef).ThenInclude(o => o.Payments)
                       .Include(o => o.ExtraExpenses)
                       .Include(o => o.UserData.SelectedWBAPITokens)
                       .Include(o => o.WBAPIKeys)
                       .FirstOrDefault(o => (o.Phone.Equals(login) || o.Email.Equals(login)));
            if (found != null)
                found.UserData.SelectedWBAPITokens = found.WBAPIKeys.Where(o => o.IsSelected).ToList();
            return found;
        }
        public IEnumerable<UserModel> GetUsersByPage(int page)
        {
            return db.Users.AsNoTracking().Include(o => o.UserData).Skip(page * 20).ToList();
        }
 






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

        public async Task<RequestStatus> UpdatePersonalData(UserModel user)
        {
            try
            {
                var found = db.Users.FirstOrDefault(o => o.Id == user.Id);

                found.Name = user.Name;
                found.Surname = user.Surname;


                found.Phone = user.Phone;
                found.Email = user.Email;

                found.Activity = user.Activity;
                found.AvatarPath = user.AvatarPath;


                db.Users.Update(found);
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }



        public async Task<RequestStatus> ChangePassword(UserModel user)
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

        public async Task<RequestStatus> AddRef(UserModel owner,UserModel refferal)
        {
            try
            {
                owner.Referrals.Add(new Database.Models.ReferralModel
                {
                    HisRef = refferal,
                    Owner = owner
                });
                await db.SaveChangesAsync();
                return new RequestStatus("Пользователь успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        public async Task<RequestStatus> SetSubscriptionDate(UserModel owner, DateTime date)
        {
            try
            {
                owner.SubscriptionBefore = date;
                db.Users.Update(owner);
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
