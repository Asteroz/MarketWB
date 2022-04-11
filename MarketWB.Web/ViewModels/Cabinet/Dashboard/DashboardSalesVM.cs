using MarketAI.API.Models;
using MarketWB.Parsing.Models.Reports.Sales;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public class DashboardSalesVM : AbsDashboardVM
    {
        public SalesReport Report { get; set; }
    }
}
