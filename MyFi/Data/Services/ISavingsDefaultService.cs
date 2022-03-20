using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface ISavingsDefaultService
{
    Task<IEnumerable<SavingsDefault>> GetAllSavingsDefaultsAsync();
    Task<IEnumerable<SavingsDefault>> GetUserSavingsDefaultsAsync(string username);
    Task<SavingsDefault> GetSavingsDefaultByIdAsync(int id);
    Task<bool> AddSavingsDefaultAsync(SavingsDefault savingsDefault, string username);
    Task UpdateSavingsDefaultAsync(SavingsDefault savingsDefault);
    Task<IEnumerable<SavingsType>> GetSavingsTypesAsync();
    Task<bool> DeleteSavingsDefaultAsync(SavingsDefault savingsDefault);
}