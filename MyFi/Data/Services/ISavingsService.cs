using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface ISavingsService
{
    Task<IEnumerable<Savings>> GetAllSavingsAsync();
    Task<IEnumerable<Savings>> GetUserSavingsAsync(string username);
    Task<IEnumerable<Savings>> GetUserSavingsByMonthAsync(DateOnly month, string username);
    Task<Savings> GetSavingsByIdAsync(int id);
    Task<bool> AddSavingsAsync(Savings savings, string username);
    Task UpdateSavingsAsync(Savings savings);
    Task<bool> DeleteSavingsAsync(Savings savings);
    Task<decimal> GetUserMonthlySavings(DateOnly month, string username);
}