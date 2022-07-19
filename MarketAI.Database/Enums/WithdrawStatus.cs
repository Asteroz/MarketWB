using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAI.Database.Enums
{
    public enum WithdrawStatus
    {
        [Description("В рассмотрении")]
        Waiting = 1,
        [Description("Одобрено")]
        Approved = 2,
        [Description("Отказано")]
        Rejected = 3
    }
}
