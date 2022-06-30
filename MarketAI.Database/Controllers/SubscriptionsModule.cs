using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using MarketAI.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yandex.Checkout.V3;

namespace MarketAI.API.Controllers
{

    public class SubscriptionsModule 
    {
        private readonly ILogger<SubscriptionsModule> _logger;
        public readonly APIDBContext db;
        public SubscriptionsModule(ILogger<SubscriptionsModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        public IEnumerable<SubscriptionModel> GetSubscriptions()
        {
            return db.Subscriptions.Include(o => o.Descriptions).ToList();
        }

        #region Админская часть
        public async Task<RequestStatus> RemoveSubscription(int id)
        {
            var subscription = db.Subscriptions.First(o => o.Id == id);
            if (subscription == null)
                return new RequestStatus("Тарифного плана с таким id не существует", 404);
            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();
            return new RequestStatus("Тарифный план успешно удален");
        }
        public async Task<RequestStatus> CreateSubscription(SubscriptionModel subscription)
        {
            try
            {
                db.Subscriptions.Add(subscription);
                await db.SaveChangesAsync();
                return new RequestStatus("Тарифный план успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        public async Task<RequestStatus> UpdateSubscription(int id,SubscriptionModel subscription)
        {
            try
            {
                var sub = db.Subscriptions.FirstOrDefault(o => o.Id == id);
                sub.Days = subscription.Days;
                sub.Price = subscription.Price;
                sub.Descriptions = subscription.Descriptions;
                db.Subscriptions.Update(sub);
               
                await db.SaveChangesAsync();
                return new RequestStatus("Тарифный план успешно обновлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }







        public GlobalSettings GetRefSettings()
        {
            return db.GlobalSettings.FirstOrDefault();
        }
        public async Task UpdateRefSettings(GlobalSettings settings)
        {
            try
            {
                db.GlobalSettings.Update(settings);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Оплата
        public async Task<string> OpenTransaction(UserModel user,SubscriptionModel subscription)
        {
            AsyncClient client = new AsyncClient(null);

            PromocodeModel userPromocode = db.Promocodes.FirstOrDefault(o => o.Code == user.ActivatedPromocode);

            double totalPrice = subscription.Price;
            if(!user.WasPromocodeActivated && userPromocode != null)
            {
                totalPrice -= totalPrice * 100 / userPromocode.Percent;
            }

            var payment = new NewPayment
            {
                Amount = new Amount()
                {
                    Currency = "RUB",
                    Value = (decimal)totalPrice
                },
                Description = $"Оплата подписки \"{subscription.Title}\"",
            };
            var createdPayment = await client.CreatePaymentAsync(payment);
            return createdPayment.Id;
        }
        public async Task<bool> CheckForPayment(UserModel user, SubscriptionModel subscription,string paymentId)
        {
            AsyncClient client = new AsyncClient(null);

            var payment = await client.GetPaymentAsync(paymentId);
            if (payment.Paid)
            {
                user.SubscriptionBefore = user.SubscriptionBefore.AddDays(subscription.Days);
                user.WasPromocodeActivated = true;
            }

            return payment.Paid;
        }



        #endregion
    }
}
