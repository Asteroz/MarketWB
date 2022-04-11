using MarketAI.API.Models;
using MarketWB.Parsing.Models.Reports.Orders;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public class DashboardOrdersVM : AbsDashboardVM
    {
        public OrdersReport Report { get; set; }
    }
}
