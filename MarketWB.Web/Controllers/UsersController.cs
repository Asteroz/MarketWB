using MarketAI.API.Enums;
using MarketAI.API.Models;
using MarketWB.Web.Models.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Users = MarketAI.API.Controllers.UsersModule;
using Stats = MarketAI.API.Controllers.StatsModule;
using Subscriptions = MarketAI.API.Controllers.SubscriptionsModule;
using MarketWB.Web.Helpers;
using MarketAI.API.Controllers;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MarketWB.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Users _api;
        private readonly Stats _stats;
        private readonly Subscriptions _subs;
        private readonly IWebHostEnvironment _appEnvironment;
        public UsersController(ILogger<UsersController> logger, 
                                Users api,
                                Stats stats,
                                Subscriptions subs,
                                IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _api = api;
            _subs = subs;
            _stats = stats;
            _appEnvironment = appEnvironment;
        }
        [Route("CheckUser")]
        [HttpGet]
        public async Task<IActionResult> CheckUser(string login)
        {
           var user = _api.GetUserByPhoneOrEmail(login);
            if (user != null)
                return new JsonResult(403);
            else
                return new JsonResult(200);
        }
        [Route("CheckUserCredentials")]
        [HttpGet]
        public async Task<IActionResult> CheckUserCredentials(string login,string password)
        {
            var user = _api.GetUserByCredintials(login, password);
            if (user != null)
                return new JsonResult(200);
            else
                return new JsonResult(404);
        }




        [HttpPost]
        public async Task<IActionResult> Auth(LoginViewModel model)
        {
            var found = _api.GetUserByCredintials(model.Login, model.Password);
            if(found != null)
            {
                  var principal = await MakeAuth(found);
                  var user = await UserHelper.GetUser(principal);
                  _stats.SetUserOnline(user, true);
                   await _stats.AddAuthStats(user);

                if (found.UserRole == MarketAI.API.Enums.UserRole.User)
                    return RedirectToAction("Dashboard", "Dashboard");
                else if (found.UserRole == MarketAI.API.Enums.UserRole.Admin)
                    return RedirectToAction("Visitors","Stats");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserModel model,[FromForm]string refcode = null)
        {
            var found = _api.GetUserByPhoneOrEmail(model.Phone);
            if(found != null)
                return RedirectToAction("Registration","Home");

            found = _api.GetUserByPhoneOrEmail(model.Email);
            if (found != null)
                return RedirectToAction("Registration", "Home");

            await _api.CreateUser(model);

            if(!string.IsNullOrEmpty(refcode))
            {
                var refUser = _api.GetUserByRefCode(refcode);
                if(refUser != null)
                {
                    await _api.AddRef(refUser,model);
                    var refSettings = _subs.GetRefSettings();
                    if(refSettings.NewRefDaysGift > 0)
                    {
                        await _api.SetSubscriptionDate(model, DateTime.Now.AddDays(refSettings.NewRefDaysGift));
                    }

                }
            }

            return RedirectToAction("Dashboard", "Dashboard");
        }
        [HttpPost]
        public async Task<IActionResult> Restore(UserModel model)
        {
            var found = _api.GetUserByPhoneOrEmail(model.Phone);
            if (found == null)
                return RedirectToAction("Restore", "Home");

            found.Password = model.Password;
            await _api.ChangePassword(found);

            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var user = await UserHelper.GetUser(User);
            _stats.SetUserOnline(user, false);

            return RedirectToAction("Login", "Home");
        }



        [Route("SendSMSCode")]
        [HttpGet]
        public async Task<IActionResult> SendSMSCode(string toPhone)
        {
            await _api.SendSMSCode(toPhone);
            return new JsonResult("Код жіберілді");

        }
        [Route("GetSMSCode")]
        [HttpGet]
        public async Task<IActionResult> GetSMSCode(string toPhone)
        {
           return new JsonResult(await _api.GetSMSCode(toPhone));
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task ChangePassword([FromBody]UserModel user)
        {
             await _api.ChangePassword(user);
        }
        [Route("ActivatePromocode")]
        [HttpPost]
        public async Task ActivatePromocode([FromBody] UserModel user)
        {
            await _api.ActivatePromocode(user);
        }


        [Route("UpdateUser")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromForm] UserModel model)
        {
            await SetAttachmentIfHas(model);
            await _api.UpdatePersonalData(model);
            return RedirectToAction("Profile", "Cabinet");
        }





        private async Task SetAttachmentIfHas(UserModel user)
        {
            var attachment = Request.Form.Files.FirstOrDefault();
            var filePath = "/uploads/" + Guid.NewGuid().ToString();

            if (attachment != null)
            {
                user.AvatarPath = filePath + attachment.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + user.AvatarPath, FileMode.Create))
                {
                    await attachment.CopyToAsync(fileStream);
                }
            }
        }

        private async Task<ClaimsPrincipal> MakeAuth(UserModel user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           
            // создаем один claim
            var claims = new List<Claim>
            {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                 new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var principal = new ClaimsPrincipal(id);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return principal;
        }

        private void MakeUser(UserRole role)
        {
            _api.CreateUser(new UserModel
            {
                UserRole = role,
                Name = "Артур",
                Email = "kachokabricosyan@yandex.ru",
                Password = "222222",
                Phone = "77054627306",
                SubscriptionBefore = DateTime.MaxValue,
            });
        }


        

    }
}
