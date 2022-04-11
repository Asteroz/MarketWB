using MarketAI.API.Models;
using MarketWB.Parsing;
using MarketWB.Parsing.Models.Reports.Orders;
using MarketWB.Parsing.Models.Reports.Rejects;
using MarketWB.Parsing.Models.Reports.Returns;
using MarketWB.Parsing.Models.Reports.Sales;
using MarketWB.Web.Helpers;
using MarketWB.Web.ViewModels.Cabinet.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildberriesAPI;
using WildberriesAPI.Models;
using SelfCostsAPI = MarketAI.API.Controllers.SelfCostsController;
using ExtraExpensesAPI = MarketAI.API.Controllers.ExtraExpensesController;
using UsersAPI = MarketAI.API.Controllers.UsersController;
using MarketAI.API.Models.WB;
using MarketWB.Parsing.Models;

namespace MarketWB.Web.Controllers.Cabinet
{
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly SelfCostsAPI _selfCostApi;
        private readonly UsersAPI _usersApi;
        private readonly ExtraExpensesAPI _extraExpensesAPI;

        public DashboardController(ILogger<DashboardController> logger,
                                                SelfCostsAPI selfCostApi,
                                                UsersAPI usersApi,
                                                ExtraExpensesAPI extraExpensesAPI)
        {
            _logger = logger;
            _selfCostApi = selfCostApi;
            _usersApi = usersApi;
            _extraExpensesAPI = extraExpensesAPI;
        }
        [Route("Cabinet/Dashboard/Analysis")]
        public IActionResult Analysis()
        {
            return View("Views/Cabinet/Dashboard/Analysis.cshtml");
        }


        [Route("Cabinet/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            var vm = new DashboardVM
            {
                DashboardReport = await MarketWBParser.GenerateDashboardReport(user.UserData),
            };
            vm = (DashboardVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/Dashboard.cshtml", vm);
        }

        [Route("Cabinet/Dashboard/Returns")]
        public async Task<IActionResult> Returns()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");

            await InitSelectedTokenIfNull(user);

            ReturnsReport report = null;
            if (user.UserData.SelectedWBAPIToken != null)
            {
                report = await MarketWBParser.GenerateReturnsReport(user.UserData);
            }
            var vm = new DashboardReturnsVM
            {
                Report = report,
            };
            vm = (DashboardReturnsVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/Returns.cshtml", vm);
        }
        [Route("Cabinet/Dashboard/Orders")]
        public async Task<IActionResult> Orders()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            OrdersReport report = null;
            if (user.UserData.SelectedWBAPIToken != null)
            {
                report = await MarketWBParser.GenerateOrdersReport(user.UserData);
            }
            var vm = new DashboardOrdersVM
            {
                Report = report,
            };
            vm = (DashboardOrdersVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/Orders.cshtml", vm);
        }
        [Route("Cabinet/Dashboard/Sales")]
        public async Task<IActionResult> Sales()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            SalesReport report = null;
            if(user.UserData.SelectedWBAPIToken != null)
            {
                report = await MarketWBParser.GenerateSalesReport(user.UserData);
            }
            var vm = new DashboardSalesVM
            {
                Report = report,
            };
            vm = (DashboardSalesVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/Sales.cshtml", vm);
        }
        [Route("Cabinet/Dashboard/Cancels")]
        public async Task<IActionResult> Cancels()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            RejectsReport report = null;

            if (user.UserData.SelectedWBAPIToken != null)
            {
                report = await MarketWBParser.GenerateRejectsReport(user.UserData);
            }
            var vm = new DashboardCancelsVM
            {
                Report = report,
            };
            vm = (DashboardCancelsVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/Cancels.cshtml", vm);
        }

        /// <summary>
        /// Ставим токен, если еще не выбран (null)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task InitSelectedTokenIfNull(UserModel user)
        {
            if (user.UserData.SelectedWBAPIToken is null)
            {
                user.UserData.SelectedWBAPIToken = user.WBAPIKeys.FirstOrDefault();
                await _usersApi.UpdateUser(user); 
            }
        }

        #region Себестоимости
        [Route("Cabinet/Dashboard/SelfCosts")]
        public async Task<IActionResult> SelfCosts()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            var vm = new DashboardSelfCostsVM
            {
                Token = user.UserData.SelectedWBAPIToken,
            };
            InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/SelfCosts.cshtml", vm);
        }

        [HttpPut]
        [Route("Cabinet/Dashboard/AddSelfCost")]
        public async Task<IActionResult> AddSelfCost()
        {
            var user = await UserHelper.GetUser(User);
            var token = user.UserData.SelectedWBAPIToken;
            if (token == null) return new JsonResult(0);
            
            return new JsonResult(await _selfCostApi.CreateSelfCost(token));
        }
        [HttpDelete]
        [Route("Cabinet/Dashboard/DeleteSelfCost")]
        public async Task DeleteSelfCost(int id)
        {
            var user = await UserHelper.GetUser(User);
            var token = user.UserData.SelectedWBAPIToken;
            if (token == null) return;

            await _selfCostApi.RemoveSelfCost(token, id);
        }
        [HttpPost]
        [Route("Cabinet/Dashboard/UpdateSelfCost")]
        public async Task UpdateSelfCost([FromBody]SelfCostModel model)
        {
            await _selfCostApi.UpdateSelfCost(model);
        }
        #endregion

        #region Дополнительные расходы
        [Route("Cabinet/Dashboard/ExtraExpenses")]
        public async Task<IActionResult> ExtraExpenses()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");
            await InitSelectedTokenIfNull(user);

            var vm = new DashboardExtraExpensesVM
            {
                Token = user.UserData.SelectedWBAPIToken,
            };
            vm = (DashboardExtraExpensesVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/ExtraExpenses.cshtml", vm);
        }

        [HttpPut]
        [Route("Cabinet/Dashboard/AddExtraExpence")]
        public async Task<IActionResult> AddExtraExpence()
        {
            var user = await UserHelper.GetUser(User);
            var token = user.UserData.SelectedWBAPIToken;
            if (token == null) return new JsonResult(0);

            return new JsonResult(await _extraExpensesAPI.CreateExtraExpense(token));
        }
        [HttpDelete]
        [Route("Cabinet/Dashboard/DeleteExtraExpence")]
        public async Task DeleteExtraExpence(int id)
        {
            var user = await UserHelper.GetUser(User);
            var token = user.UserData.SelectedWBAPIToken;
            if (token == null) return;

            await _extraExpensesAPI.RemoveExtraExpense(token, id);
        }
        [HttpPost]
        [Route("Cabinet/Dashboard/UpdateExtraExpence")]
        public async Task UpdateExtraExpence([FromBody] ExtraExpenseModel model)
        {
            await _extraExpensesAPI.UpdateExtraExpense(model);
        }
        #endregion


        private async Task<AbsDashboardVM> InitVM(UserModel user,AbsDashboardVM vm)
        {
            vm.User = user;
            vm.Brands = await MarketWBParser.GetWBBrands(user.UserData.SelectedWBAPIToken);
            vm.Categories = await MarketWBParser.GetWBCategories(user.UserData.SelectedWBAPIToken);
            vm.Brands.Insert(0,new AvailableWBBrand() { Brand = "Все бренды" });
            vm.Categories.Insert(0,new AvailableWBCategory() { Category = "Все категории" });
            return vm;
        }
    }

}
