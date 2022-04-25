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
using SelfCostsAPI = MarketAI.API.Controllers.SelfCostsModule;
using ExtraExpensesAPI = MarketAI.API.Controllers.ExtraExpensesModule;
using UsersAPI = MarketAI.API.Controllers.UsersModule;
using MarketAI.API.Models.WB;
using MarketWB.Parsing.Models;
using MarketWB.Web.Jobs;

namespace MarketWB.Web.Controllers.Cabinet
{
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly SelfCostsAPI _selfCostApi;
        private readonly UsersAPI _usersApi;
        private readonly ExtraExpensesAPI _extraExpensesAPI;
        private readonly MarketWBParser _wbparser;
        public DashboardController(ILogger<DashboardController> logger,
                                                SelfCostsAPI selfCostApi,
                                                UsersAPI usersApi,
                                                ExtraExpensesAPI extraExpensesAPI,
                                                MarketWBParser wbparser)
        {
            _logger = logger;
            _selfCostApi = selfCostApi;
            _usersApi = usersApi;
            _extraExpensesAPI = extraExpensesAPI;
            _wbparser = wbparser;
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

            var vm = new DashboardVM
            {
                DashboardReport = _wbparser.GenerateDashboardReport(user),
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



            ReturnsReport report = _wbparser.GenerateReturnsReport(user);
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


            OrdersReport report = _wbparser.GenerateOrdersReport(user);
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

            SalesReport report = _wbparser.GenerateSalesReport(user);
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


            RejectsReport report = _wbparser.GenerateRejectsReport(user);
        
            var vm = new DashboardCancelsVM
            {
                Report = report,
            };
            vm = (DashboardCancelsVM)await InitVM(user, vm);



            return View("Views/Cabinet/Dashboard/Cancels.cshtml", vm);
        }


        #region Себестоимости
        [Route("Cabinet/Dashboard/SelfCosts")]
        public async Task<IActionResult> SelfCosts()
        {
            var user = await UserHelper.GetUser(User);
            if (user.IsSubscriptionEnded)
                return RedirectToAction("NeedToPay", "Cabinet");

            await WBParsing.RefreshArticles(user);

            var vm = new DashboardSelfCostsVM();
            InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/SelfCosts.cshtml", vm);
        }

        [HttpPut]
        [Route("Cabinet/Dashboard/AddSelfCost")]
        public async Task<IActionResult> AddSelfCost()
        {
            var user = await UserHelper.GetUser(User);
            return new JsonResult(await _selfCostApi.CreateSelfCost(user));
        }
        [HttpDelete]
        [Route("Cabinet/Dashboard/DeleteSelfCost")]
        public async Task DeleteSelfCost(int id)
        {
            var user = await UserHelper.GetUser(User);
            await _selfCostApi.RemoveSelfCost(user, id);
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

            var vm = new DashboardExtraExpensesVM();
            vm = (DashboardExtraExpensesVM)await InitVM(user, vm);
            return View("Views/Cabinet/Dashboard/ExtraExpenses.cshtml", vm);
        }

        [HttpPut]
        [Route("Cabinet/Dashboard/AddExtraExpence")]
        public async Task<IActionResult> AddExtraExpence()
        {
            var user = await UserHelper.GetUser(User);
            return new JsonResult(await _extraExpensesAPI.CreateExtraExpense(user));
        }
        [HttpDelete]
        [Route("Cabinet/Dashboard/DeleteExtraExpence")]
        public async Task DeleteExtraExpence(int id)
        {
            await _extraExpensesAPI.RemoveExtraExpense(id);
        }
        [HttpPost]
        [Route("Cabinet/Dashboard/UpdateExtraExpence")]
        public async Task UpdateExtraExpence([FromBody] ExtraExpenseModel model)
        {
            await _extraExpensesAPI.UpdateExtraExpense(model);
        }
        #endregion

        [HttpPut]
        [Route("Cabinet/SetSelfBuyStatus")]
        public async Task SetSelfBuyStatus(long nmId,bool isSelfBuy)
        {
              await WBParsing.SetSelfBuyStatus(nmId, isSelfBuy);
        }


        [Route("Cabinet/WaitForParsing")]
        public async Task<IActionResult> WaitForParsing()
        {
            var user = await UserHelper.GetUser(User);
            AbsDashboardVM vm = new DashboardVM();
            vm = await InitVM(user, vm);
            return View("Views/Cabinet/WaitForParsing.cshtml", vm);
        }

        private async Task<AbsDashboardVM> InitVM(UserModel user,AbsDashboardVM vm)
        {
            vm.User = user;
            vm.Brands = _wbparser.GetWBBrands(user.UserData.SelectedWBAPITokens);
            vm.Categories = _wbparser.GetWBCategories(user.UserData.SelectedWBAPITokens);
            vm.Brands.Insert(0,new AvailableWBBrand() { Brand = "Все бренды" });
            vm.Categories.Insert(0,new AvailableWBCategory() { Category = "Все категории" });
            return vm;
        }
        private bool CheckTokenForNew(WBAPITokenModel token)
        {
            if(token == null) return false;
            if (token.CreatedFirstTime)
            {
                if (token.CreatedAt.AddMinutes(7) >= DateTime.UtcNow)
                    return true;
            }
            return false;
        }
    }

}
