using System;
using Microsoft.EntityFrameworkCore;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext context;

    public ExpenseService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<Expense>> GetAllPersonalExpensesAsync() => await context.Expenses
        .Where(e => !e.Shared).Include(e => e.Type).OrderBy(e => e.Date).ToListAsync();

    public async Task<IEnumerable<Expense>> GetUserExpensesAsync(string username) => await context.Expenses
        .Where(e => e.Username == username && !e.Shared).Include(e => e.Type).OrderBy(e => e.Date).ToListAsync();

    public async Task<IEnumerable<Expense>> GetUserExpensesByDateAsync(DateOnly startDate, DateOnly endDate, string username) {
        return await context.Expenses.Where(e => e.Username == username && !e.Shared
            && startDate <= e.Date && e.Date <= endDate).Include(e => e.Type).OrderBy(e => e.Date).ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetAllSharedExpensesAsync() => await context.Expenses
        .Where(e => e.Shared).Include(e => e.Type).OrderBy(e => e.Date).ToListAsync();

    public async Task<IEnumerable<Expense>> GetSharedExpensesByDateAsync(DateOnly startDate, DateOnly endDate) {
        return await context.Expenses.Where(e => e.Shared
            && startDate <= e.Date && e.Date <= endDate).Include(e => e.Type).OrderBy(e => e.Date).ToListAsync();
    }

    public async Task<Expense> GetExpenseByIdAsync(int id) => await context.Expenses
        .Include(e => e.Type).SingleAsync(e => e.Id == id);

    public async Task<bool> AddExpenseAsync(Expense expense, string username) {
        expense.Username = username;
        await context.Expenses.AddAsync(expense);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateExpenseAsync(Expense expense) {
        var result = await context.Expenses.SingleAsync(e => e.Id == expense.Id);
        if (result == null) {
            throw new Exception($"Expense of ID {expense.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(expense);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteExpenseAsync(Expense expense) {
        context.Expenses.Remove(expense);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync() =>
        await context.ExpenseTypes.OrderBy(e => e.Id).ToListAsync();

    public async Task<decimal> GetUserMonthlyPersonalExpenses(DateOnly startDate, DateOnly endDate, string username) {
        var e = await GetUserExpensesByDateAsync(startDate, endDate, username);
        return e.Sum(e => e.Amount);
    }

    public async Task<decimal> GetMonthlySharedExpenses(DateOnly startDate, DateOnly endDate, double split) {
        var e = await GetSharedExpensesByDateAsync(startDate, endDate);
        return (decimal)((double)e.Sum(e => e.Amount) * split);
    }
}