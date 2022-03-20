using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetAllPersonalExpensesAsync();
    Task<IEnumerable<Expense>> GetUserExpensesAsync(string username);
    Task<IEnumerable<Expense>> GetUserExpensesByDateAsync(DateOnly startDate, DateOnly endDate, string username);
    Task<IEnumerable<Expense>> GetAllSharedExpensesAsync();
    Task<IEnumerable<Expense>> GetSharedExpensesByDateAsync(DateOnly startDate, DateOnly endDate);
    Task<Expense> GetExpenseByIdAsync(int id);
    Task<bool> AddExpenseAsync(Expense expense, string username);
    Task UpdateExpenseAsync(Expense expense);
    Task<bool> DeleteExpenseAsync(Expense expense);
    Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync();
    Task<decimal> GetUserMonthlyPersonalExpenses(DateOnly startDate, DateOnly endDate, string username);
    Task<decimal> GetMonthlySharedExpenses(DateOnly startDate, DateOnly endDate, double split);
}