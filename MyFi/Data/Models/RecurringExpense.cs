using System;
using System.ComponentModel.DataAnnotations;

namespace MyFi.Data.Models;
public class RecurringExpense
{
    public int Id { get; set; }

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue)]
    public int RecurringExpenseDefaultId { get; set; }

    public RecurringExpenseDefault Default { get; set; } = default!;

    public DateOnly Month { get; set; }

    public string Username { get; set; } = default!;

    public RecurringExpense(DateOnly month) {
        Month = month;
    }

    public RecurringExpense(RecurringExpenseDefault def, DateOnly month) {
        RecurringExpenseDefaultId = def.Id;
        Amount = def.DefaultAmount.Value;
        Month = month;
    }
}