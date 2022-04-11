﻿using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yandex.Checkout.V3;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ILogger<SubscriptionsController> _logger;
        public SubscriptionsController(ILogger<SubscriptionsController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<SubscriptionModel> GetSubscriptions()
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.Subscriptions.ToList();
            }
        }

        #region Админская часть
        [HttpDelete]
        public async Task<RequestStatus> RemoveSubscription(int id)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var subscription = db.Subscriptions.First(o => o.Id == id);
                if (subscription == null)
                    return new RequestStatus("Тарифного плана с таким id не существует", 404);
                db.Subscriptions.Remove(subscription);
                await db.SaveChangesAsync();
            }
            return new RequestStatus("Тарифный план успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreateSubscription(SubscriptionModel subscription)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    db.Subscriptions.Add(subscription);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тарифный план успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateSubscription(int id,SubscriptionModel subscription)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var sub = db.Subscriptions.FirstOrDefault(o => o.Id == id);
                    sub.Days = subscription.Days;
                    sub.Price = subscription.Price;
                    db.Subscriptions.Update(sub);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Тарифный план успешно обновлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        #endregion

        #region Оплата
        [HttpPost]
        public async Task<string> OpenTransaction(UserModel user, [FromQuery] SubscriptionModel subscription)
        {
            AsyncClient client = new AsyncClient(null);

            PromocodeModel userPromocode = null;
            using(APIDBContext db = new APIDBContext())
            {
                userPromocode = db.Promocodes.FirstOrDefault(o => o.Code == user.ActivatedPromocode);
            }

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
        [HttpPost]
        public async Task<bool> CheckForPayment(UserModel user, [FromQuery] SubscriptionModel subscription,string paymentId)
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
