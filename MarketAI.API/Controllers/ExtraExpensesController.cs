﻿using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using MarketAI.API.Models.WB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExtraExpensesController : ControllerBase
    {
        private readonly ILogger<ExtraExpensesController> _logger;
        public ExtraExpensesController(ILogger<ExtraExpensesController> logger)
        {
            _logger = logger;
        }


        [HttpDelete]
        public async Task<RequestStatus> RemoveExtraExpense(WBAPITokenModel token, int id)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var expense = token.ExtraExpenses.FirstOrDefault(o => o.Id == id);
                token.ExtraExpenses.Remove(expense);

                db.ExtraExpenses.Remove(expense);

                db.WBAPITokens.Update(token);
                await db.SaveChangesAsync();
                return new RequestStatus("Запись успешно удалена");
            }
        }
        [HttpPut]
        public async Task<int> CreateExtraExpense(WBAPITokenModel token)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var expense = new ExtraExpenseModel();
                expense.WBAPITokenId = token.Id;
                token.ExtraExpenses.Add(expense);

                db.WBAPITokens.Update(token);
                await db.SaveChangesAsync();
                return expense.Id;
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdateExtraExpense(ExtraExpenseModel model)
        {
            using (APIDBContext db = new APIDBContext())
            {
                var item = db.ExtraExpenses.FirstOrDefault(o => o.Id == model.Id);
                item.Period = model.Period;
                item.ClearingCentre = model.ClearingCentre;
                item.Category = model.Category;
                item.Sum = model.Sum;
                item.Comment = model.Comment;
                item.PaymentDate = item.PaymentDate;
                item.PaymentDescription = model.PaymentDescription;
                item.PaymentReceiver = model.PaymentReceiver;

                db.ExtraExpenses.Update(item);
                await db.SaveChangesAsync();
            }
            return new RequestStatus("Запись успешно обновлена");
        }
    }
}
