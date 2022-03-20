using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyFi.Data.Models;
[Index(nameof(Description), IsUnique = true)]
public class SavingsDefault
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;

    public IEnumerable<Savings> Savings { get; set; } = new List<Savings>();

    [Range(.01, (double)decimal.MaxValue)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal? DefaultAmount { get; set; }

    public string? Website { get; set; }

    [Range(1, int.MaxValue)]
    public int SavingsTypeId { get; set; }

    public SavingsType Type { get; set; } = default!;

    public DateOnly LatestAutoAddedMonth { get; set; }

    public string Username { get; set; } = default!;
}

[Index(nameof(Description), IsUnique = true)]
public class SavingsType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description field is required.")]
    [StringLength(maximumLength: 100, MinimumLength = 1)]
    public string Description { get; set; } = default!;
}