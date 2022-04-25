using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly APIDBContext db;
        public PromocodeController(ILogger<PromocodeController> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }

        [HttpGet]
        public IEnumerable<PromocodeModel> GetPromocodes()
        {
            return db.Promocodes.AsNoTracking().ToList();
        }
        [HttpGet]
        public PromocodeModel GetPromocode(int id)
        {
            return db.Promocodes.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }
        [HttpDelete]
        public async Task<RequestStatus> RemovePromocode(int id)
        {
            var promocode = db.Promocodes.First(o => o.Id == id);
            if (promocode == null)
                return new RequestStatus("Промокода с таким id не существует", 404);
            db.Promocodes.Remove(promocode);
            await db.SaveChangesAsync();
            return new RequestStatus("Промокод успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreatePromocode(PromocodeModel promocode)
        {
            try
            {
                db.Promocodes.Add(promocode);
                await db.SaveChangesAsync();
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
                var promocode = db.Promocodes.FirstOrDefault(o => o.Id == id);
                promocode.Percent = updatedPromocode.Percent;
                promocode.Code = updatedPromocode.Code;

                db.Promocodes.Update(promocode);
                await db.SaveChangesAsync();
                return new RequestStatus("Пост успешно обновлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }
    }
}
