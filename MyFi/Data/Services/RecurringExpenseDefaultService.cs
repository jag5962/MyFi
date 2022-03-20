using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyFi.Data;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class RecurringExpenseDefaultService : IRecurringExpenseDefaultService
{
    private readonly ApplicationDbContext context;

    public RecurringExpenseDefaultService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<RecurringExpenseDefault>> GetAllRecurringExpenseDefaultsAsync() => await context.RecurringExpenseDefaults
        .Include(r => r.Type).Include(r => r.RecurringExpenses).OrderBy(r => r.Description).ToListAsync();

    public async Task<IEnumerable<RecurringExpenseDefault>> GetUserRecurringExpenseDefaultsAsync(string username) =>
        await context.RecurringExpenseDefaults.Where(r => r.Username == username || r.Shared)
        .Include(r => r.Type).Include(r => r.RecurringExpenses).OrderBy(r => r.Description).ToListAsync();

    public async Task<RecurringExpenseDefault> GetRecurringExpenseDefaultByIdAsync(int id) => await context.RecurringExpenseDefaults
            .SingleAsync(r => r.Id == id);

    public async Task<bool> AddRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault, string username) {
        recurringExpenseDefault.Username = username;
        await context.RecurringExpenseDefaults.AddAsync(recurringExpenseDefault);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault) {
        var result = await context.RecurringExpenseDefaults.SingleAsync(r => r.Id == recurringExpenseDefault.Id);
        if (result == null) {
            throw new Exception($"Recurring Expense Default of ID {recurringExpenseDefault.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(recurringExpenseDefault);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RecurringExpenseType>> GetRecurringExpenseTypesAsync()
        => await context.RecurringExpenseTypes.OrderBy(t => t.Id).ToListAsync();

    public async Task<bool> DeleteRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault) {
        context.RecurringExpenseDefaults.Remove(recurringExpenseDefault);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }
}