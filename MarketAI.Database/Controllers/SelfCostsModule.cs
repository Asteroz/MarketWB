using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class SelfCostsModule
    {
        private readonly ILogger<SelfCostsModule> _logger;
        private readonly APIDBContext db;
        public SelfCostsModule(ILogger<SelfCostsModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public async Task<RequestStatus> RemoveSelfCost(UserModel user, int selfCostId)
        {
            var selfCost = user.SelfCosts.FirstOrDefault(o => o.Id == selfCostId);
            user.SelfCosts.Remove(selfCost);

            db.SelfCosts.Remove(selfCost);

            db.Users.Update(user);
            await db.SaveChangesAsync();
            return new RequestStatus("Запись успешно удалена");
        }
        public async Task<int> CreateSelfCost(UserModel user)
        {
            var selfCost = new SelfCostModel();
            selfCost.OwnerId = user.Id;
            user.SelfCosts.Add(selfCost);

            db.Users.Update(user);
            await db.SaveChangesAsync();
            return selfCost.Id;
        }
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
