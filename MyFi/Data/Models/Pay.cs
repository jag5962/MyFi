using System;
using System.ComponentModel.DataAnnotations;

namespace MyFi.Data.Models;
public class Pay
{
    public int Id { get; set; }

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue)]
    public int IncomeId { get; set; }

    public Income IncomeSource { get; set; } = default!;

    public DateOnly Date { get; set; }

    public string Username { get; set; } = default!;

    public Pay(Income source, DateOnly date) {
        IncomeId = source.Id;
        Amount = source.DefaultPay.Value;
        Date = date;
    }

    public Pay(DateOnly date) {
        Date = date;
    }
}