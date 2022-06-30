using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using MarketAI.API.Models.WB;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    public class ExtraExpensesModule
    {
        private readonly ILogger<ExtraExpensesModule> _logger;
        private readonly APIDBContext db;
        public ExtraExpensesModule(ILogger<ExtraExpensesModule> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public async Task<RequestStatus> RemoveExtraExpense(int id)
        {
            var expense = db.ExtraExpenses.FirstOrDefault(o => o.Id == id);
            db.ExtraExpenses.Remove(expense);
            await db.SaveChangesAsync();
            return new RequestStatus("Запись успешно удалена");
        }
        public async Task<int> CreateExtraExpense(UserModel user)
        {
            var expense = new ExtraExpenseModel();
            expense.OwnerId = user.Id;
            user.ExtraExpenses.Add(expense);

            db.Users.Update(user);

            await db.SaveChangesAsync();
            return expense.Id;
        }
        public async Task<int> CreateExtraExpense(UserModel user, ExtraExpenseModel expense)
        {
            expense.OwnerId = user.Id;
            user.ExtraExpenses.Add(expense);

            db.Users.Update(user);

            await db.SaveChangesAsync();
            return expense.Id;
        }
        public async Task<RequestStatus> UpdateExtraExpense(ExtraExpenseModel model)
        {
            var item = db.ExtraExpenses.FirstOrDefault(o => o.Id == model.Id);
            item.Period = model.Period;
            item.ClearingCentre = model.ClearingCentre;
            item.Category = model.Category;
            item.Sum = model.Sum;
            item.Comment = model.Comment;
            item.PaymentDate = model.PaymentDate;
            item.PaymentDescription = model.PaymentDescription;
            item.PaymentReceiver = model.PaymentReceiver;

            db.ExtraExpenses.Update(item);
            await db.SaveChangesAsync();
            return new RequestStatus("Запись успешно обновлена");
        }
    }
}
