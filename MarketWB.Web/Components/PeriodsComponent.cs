using MarketWB.Web.ViewModels.Cabinet.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MarketWB.Web.Views.Components
{
    public class PeriodsComponent : ViewComponent
    {

        public IViewComponentResult Invoke(AbsDashboardVM vm)
        {
            return View(vm);
        }
    }
}
