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

namespace MarketWB.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Users _api;
        public UsersController(ILogger<UsersController> logger, Users api)
        {
            _logger = logger;
            _api = api;
        }


        [HttpPost]
        public async Task<IActionResult> Auth(LoginViewModel model)
        {
            MakeUser(UserRole.User);


            var found = _api.GetUserByCredintials(model.Login, model.Password);
            if(found != null)
            {
                  MakeAuth(found);
            
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
                return View();

            found = _api.GetUserByPhoneOrEmail(model.Email);
            if (found != null)
                return View();

            await _api.CreateUser(model);

            return View("Views/Cabinet/Dashboard/Dashboard.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Restore(UserModel model)
        {
            var found = _api.GetUserByPhoneOrEmail(model.Phone);
            if (found == null)
                return View();

            found.Password = model.Password;
            await _api.UpdateUser(found);

            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }


        public async Task SendSMSCode(string toPhone)
        {

        }


        private async void MakeAuth(UserModel user)
        {
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
