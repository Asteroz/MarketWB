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
using Users = MarketAI.API.Controllers.UsersController;
using Stats = MarketAI.API.Controllers.StatsController;
using MarketWB.Web.Helpers;

namespace MarketWB.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Users _api;
        private readonly Stats _stats;
        public UsersController(ILogger<UsersController> logger, Users api,Stats stats)
        {
            _logger = logger;
            _api = api;
            _stats = stats;
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
                  MakeAuth(found);
                  var user = UserHelper.GetUser(HttpContext.User);
                  _stats.SetUserOnline(user, true);
                  _stats.AddAuthStats(user);

                if (found.UserRole == MarketAI.API.Enums.UserRole.User)
                    return View("Views/Cabinet/Dashboard/Dashboard.cshtml");
                 else if (found.UserRole == MarketAI.API.Enums.UserRole.Admin)
                    return RedirectToAction("Visitors","Stats");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            var found = _api.GetUserByPhoneOrEmail(model.Phone);
            if(found != null)
                return RedirectToAction("Registration","Home");

            found = _api.GetUserByPhoneOrEmail(model.Email);
            if (found != null)
                return RedirectToAction("Registration", "Home");

            await _api.CreateUser(model);

            return View("Views/Cabinet/Dashboard/Dashboard.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Restore(UserModel model)
        {
            var found = _api.GetUserByPhoneOrEmail(model.Phone);
            if (found == null)
                return RedirectToAction("Restore", "Home");

            found.Password = model.Password;
            await _api.UpdateUser(found);

            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var user = UserHelper.GetUser(User);
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
        public async Task<IActionResult> ChangePassword(UserModel user)
        {
             await _api.UpdateUser(user);
             return RedirectToAction("Dashboard", "Dashboard");
        }


        private async void MakeAuth(UserModel user)
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
            
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            
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
