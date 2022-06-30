using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesAPI.Comparers
{
    internal class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x == y;
        }

        public int GetHashCode([DisallowNull] DateTime obj)
        {
           return obj.GetHashCode();
        }
    }
}
