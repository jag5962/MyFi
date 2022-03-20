using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface IPayService
{
    Task<IEnumerable<Pay>> GetAllPaysAsync();
    Task<IEnumerable<Pay>> GetUserPaysAsync(string username);
    Task<IEnumerable<Pay>> GetUserPaysByDateAsync(DateOnly startDate, DateOnly endDate, string username);
    Task<Pay> GetPayByIdAsync(int id);
    Task<DateOnly> GetUserLatestPayDate(Income source, string username);
    Task<bool> AddPayAsync(Pay pay, string username);
    Task UpdatePayAsync(Pay pay);
    Task<bool> DeletePayAsync(Pay pay);
    Task<decimal> GetUserMonthlyIncome(DateOnly startDate, DateOnly endDate, string username);
}