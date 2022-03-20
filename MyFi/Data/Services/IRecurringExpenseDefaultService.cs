using System;
using MyFi.Data.Models;

namespace MyFi.Data.Services;
public interface IRecurringExpenseDefaultService
{
    Task<IEnumerable<RecurringExpenseDefault>> GetAllRecurringExpenseDefaultsAsync();
    Task<IEnumerable<RecurringExpenseDefault>> GetUserRecurringExpenseDefaultsAsync(string username);
    Task<RecurringExpenseDefault> GetRecurringExpenseDefaultByIdAsync(int id);
    Task<bool> AddRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault, string username);
    Task UpdateRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault);
    Task<IEnumerable<RecurringExpenseType>> GetRecurringExpenseTypesAsync();
    Task<bool> DeleteRecurringExpenseDefaultAsync(RecurringExpenseDefault recurringExpenseDefault);
}