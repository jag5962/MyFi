using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyFi.Data;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public class SavingsDefaultService : ISavingsDefaultService
{
    private readonly ApplicationDbContext context;

    public SavingsDefaultService(ApplicationDbContext appDbContext) {
        context = appDbContext;
    }

    public async Task<IEnumerable<SavingsDefault>> GetAllSavingsDefaultsAsync() => await context.SavingsDefaults
        .Include(s => s.Type).Include(s => s.Savings).OrderBy(s => s.Description).ToListAsync();

    public async Task<IEnumerable<SavingsDefault>> GetUserSavingsDefaultsAsync(string username) => await context.SavingsDefaults
        .Where(s => s.Username == username).Include(s => s.Type).Include(s => s.Savings)
        .OrderBy(s => s.Description).ToListAsync();

    public async Task<SavingsDefault> GetSavingsDefaultByIdAsync(int id) => await context.SavingsDefaults
            .SingleAsync(s => s.Id == id);

    public async Task<bool> AddSavingsDefaultAsync(SavingsDefault savingsDefault, string username) {
        savingsDefault.Username = username;
        await context.SavingsDefaults.AddAsync(savingsDefault);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public async Task UpdateSavingsDefaultAsync(SavingsDefault savingsDefault) {
        var result = await context.SavingsDefaults.SingleAsync(s => s.Id == savingsDefault.Id);
        if (result == null) {
            throw new Exception($"Savings Default of ID {savingsDefault.Id} not found.");
        }
        context.Entry(result).CurrentValues.SetValues(savingsDefault);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SavingsType>> GetSavingsTypesAsync()
        => await context.SavingsTypes.OrderBy(t => t.Id).ToListAsync();

    public async Task<bool> DeleteSavingsDefaultAsync(SavingsDefault savingsDefault) {
        context.SavingsDefaults.Remove(savingsDefault);
        try {
            await context.SaveChangesAsync();
            return true;
        } catch (Exception) {
            return false;
        }
    }
}