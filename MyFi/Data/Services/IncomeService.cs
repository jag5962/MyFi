using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyFi.Data;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class IncomeService : IIncomeService
{
    private readonly ApplicationDbContext context;

    public IncomeService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<Income>> GetAllIncomesAsync() => await context.Incomes
        .Include(i => i.Type).Include(i => i.Pays).OrderBy(i => i.Description).ToListAsync();

    public async Task<IEnumerable<Income>> GetUserIncomesAsync(string username) => await context.Incomes
        .Where(i => i.Username == username).Include(i => i.Type).Include(i => i.Pays)
        .OrderBy(i => i.Description).ToListAsync();

    public async Task<Income> GetIncomeByIdAsync(int id) => await context.Incomes
            .SingleAsync(i => i.Id == id);

    public async Task<bool> AddIncomeAsync(Income income, string username) {
        income.Username = username;
        await context.Incomes.AddAsync(income);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateIncomeAsync(Income income) {
        var result = await context.Incomes.SingleAsync(i => i.Id == income.Id);
        if (result == null) {
            throw new Exception($"Income of ID {income.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(income);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<IncomeType>> GetIncomeTypesAsync()
        => await context.IncomeTypes.OrderBy(t => t.Id).ToListAsync();

    public async Task<bool> DeleteIncomeAsync(Income income) {
        context.Incomes.Remove(income);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<double> GetUserSplitPercent(string username) {
        var totalGrossPay = await context.Incomes.SumAsync(i => i.AnnualGrossPay);
        if (totalGrossPay == 0)
            return 1;
        var userGrossPay = await context.Incomes.Where(i => i.Username == username)
            .SumAsync(i => i.AnnualGrossPay);
        return (double)userGrossPay / (double)totalGrossPay;
    }
}