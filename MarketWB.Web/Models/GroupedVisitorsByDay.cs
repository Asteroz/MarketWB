using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Models
{
    public class GroupedVisitorsByDay
    {
        public DateTime Date { get; set; }
        public int Visitors { get; set; }
    }
}
