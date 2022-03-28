using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyFi.Data;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class RecurringExpenseService : IRecurringExpenseService
{
    private readonly ApplicationDbContext context;

    public RecurringExpenseService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<RecurringExpense>> GetAllRecurringExpenseAsync() =>
        await context.RecurringExpenses.Include(r => r.Default).ToListAsync();

    public async Task<IEnumerable<RecurringExpense>> GetUserRecurringExpenseAsync(string username) =>
        await context.RecurringExpenses.Where(r => r.Username == username || r.Default.Shared)
        .Include(r => r.Default).ToListAsync();

    public async Task<IEnumerable<RecurringExpense>> GetUserRecurringExpenseByMonthAsync(DateOnly month, string username) =>
        await context.RecurringExpenses.Where(r => (r.Username == username || r.Default.Shared) && month == r.Month)
        .Include(r => r.Default).ToListAsync();

    public async Task<RecurringExpense> GetRecurringExpenseByIdAsync(int id) =>
        await context.RecurringExpenses.Include(r => r.Default).SingleAsync(r => r.Id == id);

    public async Task<IEnumerable<RecurringExpense>> GetAllSharedRecurringExpenseByMonthAsync(DateOnly month) =>
        await context.RecurringExpenses.Where(r => r.Default.Shared && month == r.Month)
        .Include(r => r.Default).Include(r => r.Default.Type).ToListAsync();

    public async Task<bool> AddRecurringExpenseAsync(RecurringExpense recurringExpense, string username) {
        recurringExpense.Username = username;
        await context.RecurringExpenses.AddAsync(recurringExpense);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateRecurringExpenseAsync(RecurringExpense recurringExpense) {
        var result = await context.RecurringExpenses.SingleAsync(r => r.Id == recurringExpense.Id);
        if (result == null) {
            throw new Exception($"Recurring expense of ID {recurringExpense.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(recurringExpense);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteRecurringExpenseAsync(RecurringExpense recurringExpense) {
        context.RecurringExpenses.Remove(recurringExpense);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<decimal> GetUserMonthlyRecurringExpenses(DateOnly month, string username, double split) {
        var rs = await GetUserRecurringExpenseByMonthAsync(month, username);
        double sum = 0;
        foreach (var r in rs)
            sum += (double)r.Amount * (r.Default.Shared ? split : 1);
        return (decimal)sum;
    }
}