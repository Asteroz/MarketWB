using MarketAI.API.Models;
using MarketWB.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MarketWB.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static UserModel GetUser(this Controller controller)
        {
            return UserHelper.GetUser(controller.User);
        }
    }
}
