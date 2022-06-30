using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SmsAero
{
    public class SMSAero
    {
        public SMSAero() { }
        public SMSAero(string email,string apikey) 
        {
            Email = email;
            ApiKey = apikey;
        }

        public string Email { get; set; } = "wb.pr0@yandex.ru";
        public string ApiKey { get; set; } = "XKhmj42CPORGnZrFWSmGKsvjEtc6";

        public async Task SendSMS(string number,string text,string sign)
        {
            try
            {
                string url = $"https://gate.smsaero.ru/v2/sms/send?number={number}&text={text}&sign={sign}";

                RestClient client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.Credentials = new NetworkCredential(Email, ApiKey);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                CancellationToken cancellationToken = new CancellationToken();
                var response =  await client.ExecuteAsync(request, cancellationToken);

            }
             catch (Exception ex)
            {

            }

        }
    }
}
