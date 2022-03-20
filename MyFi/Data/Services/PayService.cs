using System;
using Microsoft.EntityFrameworkCore;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class PayService : IPayService
{
    private readonly ApplicationDbContext context;

    public PayService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<Pay>> GetAllPaysAsync() => await context.Pays
        .Include(p => p.IncomeSource).OrderBy(p => p.Date).ToListAsync();

    public async Task<IEnumerable<Pay>> GetUserPaysAsync(string username) => await context.Pays
        .Where(p => p.Username == username).Include(p => p.IncomeSource).OrderBy(p => p.Date)
        .ToListAsync();

    public async Task<IEnumerable<Pay>> GetUserPaysByDateAsync(DateOnly startDate,
        DateOnly endDate, string username) {
        return await context.Pays.Where(p => p.Username == username
            && startDate <= p.Date && p.Date <= endDate).Include(p => p.IncomeSource)
            .OrderBy(p => p.Date).ToListAsync();
    }

    public async Task<Pay> GetPayByIdAsync(int id) => await context.Pays
            .SingleAsync(p => p.Id == id);

    public async Task<bool> AddPayAsync(Pay pay, string username) {
        pay.Username = username;
        await context.Pays.AddAsync(pay);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<DateOnly> GetUserLatestPayDate(Income source, string username) {
        return await context.Pays.Where(p => p.IncomeId == source.Id &&
        p.Username == username).MaxAsync(p => p.Date);
    }

    public async Task UpdatePayAsync(Pay pay) {
        var result = await context.Pays.SingleAsync(p => p.Id == pay.Id);
        if (result == null) {
            throw new Exception($"Pay of ID {pay.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(pay);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeletePayAsync(Pay pay) {
        context.Pays.Remove(pay);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<decimal> GetUserMonthlyIncome(DateOnly startDate, DateOnly endDate, string username) {
        var pays = await GetUserPaysByDateAsync(startDate, endDate, username);
        return pays.Sum(p => p.Amount);
    }
}