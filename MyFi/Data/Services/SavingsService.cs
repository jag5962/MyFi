using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyFi.Data;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class SavingsService : ISavingsService
{
    private readonly ApplicationDbContext context;

    public SavingsService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<Savings>> GetAllSavingsAsync() => await context.Savings.ToListAsync();

    public async Task<IEnumerable<Savings>> GetUserSavingsAsync(string username) => await context.Savings
        .Where(s => s.Username == username).ToListAsync();

    public async Task<IEnumerable<Savings>> GetUserSavingsByMonthAsync(DateOnly month, string username) {
        return await context.Savings.Where(s => s.Username == username && month == s.Month).ToListAsync();
    }

    public async Task<Savings> GetSavingsByIdAsync(int id) => await context.Savings
            .SingleAsync(s => s.Id == id);

    public async Task<bool> AddSavingsAsync(Savings savings, string username) {
        savings.Username = username;
        await context.Savings.AddAsync(savings);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateSavingsAsync(Savings savings) {
        var result = await context.Savings.SingleAsync(s => s.Id == savings.Id);
        if (result == null) {
            throw new Exception($"Savings of ID {savings.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(savings);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteSavingsAsync(Savings savings) {
        context.Savings.Remove(savings);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task<decimal> GetUserMonthlySavings(DateOnly month, string username) {
        var s = await GetUserSavingsByMonthAsync(month, username);
        return s.Sum(s => s.Amount);
    }
}