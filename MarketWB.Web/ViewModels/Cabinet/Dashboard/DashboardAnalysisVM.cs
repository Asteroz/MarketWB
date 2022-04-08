using MarketAI.API.Models;
using System.Collections.Generic;

namespace MarketWB.Web.ViewModels.Cabinet.Dashboard
{
    public class DashboardAnalysisVM
    {
        public UserModel User { get; set; }

        public List<string> MyBrands { get; set; } = new List<string>();
        public List<string> MySubjects { get; set; } = new List<string>();
    }
}
