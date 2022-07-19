using MarketAI.API.Core;
using MarketAI.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Yandex.Checkout.V3;

namespace MarketWB.Web.Controllers.Cabinet
{
    public class PaymentsController : Controller
    {
        private readonly APIDBContext _db;
        private const string KEY = "test_65zw6hplub_jNSuaoKLGjJM5C4b9tjLKvxXJBgBJlXk";
        private const string PAYOUT_TOKEN = "";
        private const string SHOP_ID = "926124";
        Yandex.Checkout.V3.Client _client;
        public PaymentsController(APIDBContext db)
        {
            _db = db;
            _client = new Yandex.Checkout.V3.Client(
                shopId: SHOP_ID,
                secretKey: KEY);
        }
        public async Task<IActionResult> PaySubscription(int userID,int subscriptionId)
        {
            var user = _db.Users.FirstOrDefault(o => o.Id == userID);
            var subscription = _db.Subscriptions.FirstOrDefault(o => o.Id == subscriptionId);

            if(subscription.Price < 1)
            {
                user.WasTrialActivated = true;


                if (user.IsSubscriptionEnded)
                    user.SubscriptionBefore = System.DateTime.Now.AddDays(subscription.Days);
                else
                    user.SubscriptionBefore = user.SubscriptionBefore.AddDays(subscription.Days);
                _db.Users.Update(user);
                _db.SaveChanges();
                return RedirectToAction("Dashboard", "Dashboard");
            }

            decimal sum = (decimal)subscription.Price;

            var promocode = _db.Promocodes.FirstOrDefault(o => o.Code == user.ActivatedPromocode);
            if(promocode != null)
            {
                switch (promocode.Type)
                {
                    case MarketAI.Database.Enums.PromocodeType.Percent:
                        sum -= sum / 100 * (decimal)promocode.Percent;
                        break;
                    case MarketAI.Database.Enums.PromocodeType.Value:
                        sum -= (decimal)promocode.Percent;
                        break;
                }
            }

            var newPayment = new NewPayment
            {
                Amount = new Amount { Value = sum, Currency = "RUB" },
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = $"https://feeno.ru/Webhook?userID={userID}&subscriptionId={subscriptionId}&sum={sum}",
                }
            };

            Payment payment = _client.CreatePayment(newPayment);

            string url = payment.Confirmation.ConfirmationUrl;
            Response.Redirect(url);

            return Redirect(url);

           
        }
        [Route("Webhook"),HttpGet,HttpPost,HttpPut]
        public IActionResult Webhook(int userID,int subscriptionId,decimal sum)
        {
            var user = _db.Users.FirstOrDefault(o => o.Id == userID);
            var subscription = _db.Subscriptions.FirstOrDefault(o => o.Id == subscriptionId);

            Message message = Client.ParseMessage(Request.Method, Request.ContentType, Request.Body);
            Payment payment = message?.Object;
            if (message?.Event == Event.PaymentWaitingForCapture && payment.Paid)
            {
                // 4. Подтвердите готовность принять платеж
                _client.CapturePayment(payment.Id);

                if (user.IsSubscriptionEnded)
                    user.SubscriptionBefore = System.DateTime.Now.AddDays(subscription.Days);
                else
                    user.SubscriptionBefore = user.SubscriptionBefore.AddDays(subscription.Days);

                user.Payments.Add(new MarketAI.API.Models.Stats.PaymentModel
                {
                    Date = System.DateTime.Now,
                    Sum = (double)sum,
                    User = user,
                    Description = $"Оплата тарифа {subscription.Title}"
                });

                if(subscription.Days > 29)
                {
                    var referal = _db.Referrals.FirstOrDefault(o => o.HisRefId == userID);
                    if(referal != null)
                    {
                        var refUser = _db.Users.FirstOrDefault(o => o.Id == referal.OwnerId);
                        if(refUser != null)
                        {
                            refUser.ReferralBalance += 300;
                            _db.Users.Update(refUser);
                        }
                    }
                }

                if (user.ActivatedPromocode != null)
                    user.WasPromocodeActivated = true;
            }

            _db.Users.Update(user);
            _db.SaveChanges();

            return RedirectToAction("Dashboard", "Dashboard");
        }

        public IActionResult MakeWithdrawRequest(int userID, decimal sum,string cardNumber)
        {
            var user = _db.Users.FirstOrDefault(o => o.Id == userID);
            if(user.ReferralBalance <= sum)
            {
                return new ForbidResult();
            }
            user.WithdrawRequests.Add(new MarketAI.Database.Models.WithdrawRequest
            {
                Sum = sum,
                User = user,
                AccountNumber = cardNumber
            });

            user.ReferralBalance -= sum;

            _db.Users.Update(user);
            _db.SaveChanges();

            return RedirectToAction("WithdrawHistory", "Referral");
        }
        [Route("HasUserEnoughMoney")]
        public IActionResult HasUserEnoughMoney(int userID, decimal sum)
        {
            var user = _db.Users.FirstOrDefault(o => o.Id == userID);
            return new JsonResult(user?.ReferralBalance >= sum);
        }
        public IActionResult DecidePayout(int withdrawReqId, WithdrawStatus status)
        {
            var request = _db.WithdrawRequests.FirstOrDefault(o => o.Id == withdrawReqId);
            request.Status = status;

            //if(status == WithdrawStatus.Approved)
            //{
            //    _client.CreatePayout(new NewPayout
            //    {
            //        Description = $"Выплата {(int)request.Sum} рублей за участие в реферальной программе",
            //        Amount = new Amount()
            //        {
            //            Currency = "RUB",
            //            Value = request.Sum
            //        },
            //        Deal = new PayoutDeal
            //        {
            //            Id = request.Id.ToString()
            //        },
            //        PayoutToken = PAYOUT_TOKEN,
            //    });
            //}

            _db.WithdrawRequests.Update(request);
            _db.SaveChanges();

            return RedirectToAction("WithdrawRequests", "Stats");
        }
    }
}
