using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models.Statuses
{
    public class RequestStatus
    {
        public RequestStatus()
        {

        }
        public RequestStatus(string msg)
        {
            Message = msg;
        }
        public RequestStatus(string msg,int code)
        {
            Message = msg;
            Code = code;
        }
        public int Code { get; set; } = 200;
        public string Message { get; set; }
    }
}
