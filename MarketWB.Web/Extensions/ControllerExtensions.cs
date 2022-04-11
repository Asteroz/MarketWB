using MarketAI.API.Models;
using MarketWB.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketWB.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<UserModel> GetUser(this Controller controller)
        {
            return await UserHelper.GetUser(controller.User);
        }
    }
}
