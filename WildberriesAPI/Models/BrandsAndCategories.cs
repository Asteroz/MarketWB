using System;
using System.Collections.Generic;
using System.Text;

namespace WildberriesAPI.Models
{
    public class BrandsAndCategories
    {
        public List<string> Brands { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
