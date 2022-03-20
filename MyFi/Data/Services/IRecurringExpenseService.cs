using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface IRecurringExpenseService
{
    Task<IEnumerable<RecurringExpense>> GetAllRecurringExpenseAsync();
    Task<IEnumerable<RecurringExpense>> GetUserRecurringExpenseAsync(string username);
    Task<IEnumerable<RecurringExpense>> GetUserRecurringExpenseByMonthAsync(DateOnly month, string username);
    Task<RecurringExpense> GetRecurringExpenseByIdAsync(int id);
    Task<bool> AddRecurringExpenseAsync(RecurringExpense recurringExpense, string username);
    Task UpdateRecurringExpenseAsync(RecurringExpense recurringExpense);
    Task<bool> DeleteRecurringExpenseAsync(RecurringExpense recurringExpense);
    Task<decimal> GetUserMonthlyRecurringExpenses(DateOnly month, string username, double split);
}