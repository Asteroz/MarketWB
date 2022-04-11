using MarketAI.API.Models;
using MarketWB.Parsing.Models.Reports.Rejects;
using MarketWB.Parsing.Models.Reports.Sales;
using WildberriesAPI.Models;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public class DashboardCancelsVM : AbsDashboardVM
    {
        public RejectsReport Report { get; set; }
    }
}
