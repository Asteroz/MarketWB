using MarketAI.API.Controllers;
using MarketAI.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketWB.Web.Helpers
{
    public static class UserHelper
    {
        public static async Task<UserModel> GetUser(ClaimsPrincipal principal)
        {
            UsersModule api = new UsersModule(null,new MarketAI.API.Core.APIDBContext());
           return await api.GetUserById(Convert.ToInt32(principal.Identity.Name));
        }
    }
}
