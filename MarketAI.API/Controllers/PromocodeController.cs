using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromocodeController : ControllerBase
    {
        private readonly ILogger<PromocodeController> _logger;
        public PromocodeController(ILogger<PromocodeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<PromocodeModel> GetPromocodes()
        {
            using (APIDBContext db = new APIDBContext())
            {
                return db.Promocodes.ToList();
            }
        }
        [HttpDelete]
        public async Task<RequestStatus> RemovePromocode(int id)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var promocode = db.Promocodes.First(o => o.Id == id);
                if (promocode == null)
                    return new RequestStatus("Промокода с таким id не существует", 404);
                db.Promocodes.Remove(promocode);
                await db.SaveChangesAsync();
            }
            return new RequestStatus("Промокод успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreatePromocode(PromocodeModel promocode)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var existing = db.Promocodes.FirstOrDefault(o => o.Code == promocode.Code);
                    if(existing != null)
                        return new RequestStatus("Данный промокод уже существует", 400);

                    db.Promocodes.Add(promocode);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Пост успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdatePromocode(int id, PromocodeModel updatedPromocode)
        {
            try
            {
                using (APIDBContext db = new APIDBContext())
                {
                    var promocode = db.Promocodes.First(o => o.Id == id);
                    if (promocode == null)
                        return new RequestStatus("Промокод с таким id не существует", 404);

                    updatedPromocode.Id = promocode.Id;
                    if (updatedPromocode.Code == promocode.Code)
                        return new RequestStatus("Данный промокод уже существует", 400);

                    promocode = updatedPromocode;
                    db.Promocodes.Update(promocode);
                    await db.SaveChangesAsync();
                }
                return new RequestStatus("Пост успешно обновлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
    }
}
