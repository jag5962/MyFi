using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyFi.Data.Models;
[Index(nameof(Description), IsUnique = true)]
public class Income
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;

    public IEnumerable<Pay> Pays { get; set; } = new List<Pay>();

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal? DefaultPay { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal AnnualGrossPay { get; set; }

    public string? Website { get; set; }

    [Range(1, int.MaxValue)]
    public int IncomeTypeId { get; set; }

    public IncomeType Type { get; set; } = default!;

    public DateOnly LatestAutoAddedMonth { get; set; }

    public string Username { get; set; } = default!;
}

[Index(nameof(Description), IsUnique = true)]
public class IncomeType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;
}