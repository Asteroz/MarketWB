using MarketAI.API.Models;
using MarketAI.API.Models.WB;
using System.Collections.Generic;
using WildberriesAPI.Models;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public abstract class AbsDashboardVM
    {
        public UserModel User { get; set; }
        public List<AvailableWBCategory> Categories { get; set; } = new List<AvailableWBCategory>();
        public List<AvailableWBBrand> Brands { get; set; } = new List<AvailableWBBrand>();
    }
}
