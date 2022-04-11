using MarketAI.API.Models;
using MarketWB.Parsing.Models.Reports.Orders;
using MarketWB.Parsing.Models.Reports.Returns;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public class DashboardReturnsVM : AbsDashboardVM
    {
        public ReturnsReport Report { get; set; }
    }
}
