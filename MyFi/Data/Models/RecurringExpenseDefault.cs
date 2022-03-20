using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyFi.Data.Models;
[Index(nameof(Description), IsUnique = true)]
public class RecurringExpenseDefault
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;

    public IEnumerable<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal? DefaultAmount { get; set; }

    public string? Website { get; set; }

    [Range(1, int.MaxValue)]
    public int RecurringExpenseTypeId { get; set; }

    public RecurringExpenseType Type { get; set; } = default!;

    public bool Shared { get; set; }

    public DateOnly LatestAutoAddedMonth { get; set; }

    public string Username { get; set; } = default!;
}

[Index(nameof(Description), IsUnique = true)]
public class RecurringExpenseType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;
}