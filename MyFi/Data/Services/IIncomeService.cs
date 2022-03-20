using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface IIncomeService
{
    Task<IEnumerable<Income>> GetAllIncomesAsync();
    Task<IEnumerable<Income>> GetUserIncomesAsync(string username);
    Task<Income> GetIncomeByIdAsync(int id);
    Task<bool> AddIncomeAsync(Income income, string username);
    Task UpdateIncomeAsync(Income income);
    Task<IEnumerable<IncomeType>> GetIncomeTypesAsync();
    Task<bool> DeleteIncomeAsync(Income income);
    Task<double> GetUserSplitPercent(string username);
}