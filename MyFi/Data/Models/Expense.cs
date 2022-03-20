using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyFi.Data.Models;
public class Expense
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    [Range(1, int.MaxValue)]
    public int ExpenseTypeId { get; set; }

    public ExpenseType Type { get; set; } = default!;

    public bool Shared { get; set; }

    public string Username { get; set; } = default!;

    public Expense(DateOnly date) {
        Date = date;
    }
}

[Index(nameof(Description), IsUnique = true)]
public class ExpenseType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 2)]
    public string Description { get; set; } = default!;
}