﻿using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SelfCostsController : ControllerBase
    {
        private readonly ILogger<SelfCostsController> _logger;
        private readonly APIDBContext db;
        public SelfCostsController(ILogger<SelfCostsController> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }


        [HttpDelete]
        public async Task<RequestStatus> RemoveSelfCost(WBAPITokenModel token, int selfCostId)
        {
            var selfCost = token.SelfCosts.FirstOrDefault(o => o.Id == selfCostId);
            token.SelfCosts.Remove(selfCost);

            db.SelfCosts.Remove(selfCost);

            db.WBAPITokens.Update(token);
            await db.SaveChangesAsync();
            return new RequestStatus("Запись успешно удалена");
        }
        [HttpPut]
        public async Task<int> CreateSelfCost(WBAPITokenModel token)
        {
            var selfCost = new SelfCostModel();
            selfCost.WBAPITokenId = token.Id;
            token.SelfCosts.Add(selfCost);

            db.WBAPITokens.Update(token);
            await db.SaveChangesAsync();
            return selfCost.Id;
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateSelfCost(SelfCostModel model)
        {
            var item = db.SelfCosts.FirstOrDefault(o => o.Id == model.Id);
            item.PurchaseCost = model.PurchaseCost;
            item.FFServicesCost = model.FFServicesCost;
            item.DeliveryToYourStockCost = model.DeliveryToYourStockCost;
            item.PhotographCost = model.PhotographCost;
            item.DeliveryToWbStockCost = model.DeliveryToWbStockCost;
            item.PackagingCost = model.PackagingCost;
            item.ProductId = model.ProductId;

            db.SelfCosts.Update(item);
            await db.SaveChangesAsync();
            return new RequestStatus("Запись успешно удалена");
        }
    }
}
