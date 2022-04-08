using MarketAI.API.Models;
using System.Collections.Generic;

namespace MarketWB.Web.ViewModels.Cabinet
{
    public class NewsViewModel
    {
        public IEnumerable<PostModel> Posts { get; set; }
        public List<string> Tags { get; set; }
    }
}
