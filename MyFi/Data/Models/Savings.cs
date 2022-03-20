using System;
using System.ComponentModel.DataAnnotations;

namespace MyFi.Data.Models;
public class Savings
{
    public int Id { get; set; }

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue)]
    public int SavingsDefaultId { get; set; }

    public SavingsDefault Default { get; set; } = default!;

    public DateOnly Month { get; set; }

    public string Username { get; set; } = default!;

    public Savings(SavingsDefault def, DateOnly month) {
        SavingsDefaultId = def.Id;
        Amount = def.DefaultAmount.Value;
        Month = month;
    }

    public Savings(DateOnly month) {
        Month = month;
    }
}