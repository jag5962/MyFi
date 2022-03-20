using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class RecurringExpenseEdit
{
    [Inject]
    public IRecurringExpenseService RecurringExpenseService { get; set; } = default!;
    [Inject]
    public IRecurringExpenseDefaultService DefaultService { get; set; } = default!;
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    public RecurringExpense RecurringExpense { get; set; } = new RecurringExpense(
        DateOnly.FromDateTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)));
    public IEnumerable<RecurringExpenseDefault> RecurringExpenseDefaults { get; set; } = new List<RecurringExpenseDefault>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        RecurringExpenseDefaults = await DefaultService.GetUserRecurringExpenseDefaultsAsync(username);

        if (Id != 0)
            RecurringExpense = await RecurringExpenseService.GetRecurringExpenseByIdAsync(Id);

        if (RecurringExpenseDefaults.Count() == 1)
            DefaultSelected(RecurringExpenseDefaults.Select(m => m.Id).Single());
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (RecurringExpense.Id == 0) {
            bool successful = await RecurringExpenseService.AddRecurringExpenseAsync(RecurringExpense,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New recurring expense added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new recurring expense. Please try again.";
            }
        } else {
            await RecurringExpenseService.UpdateRecurringExpenseAsync(RecurringExpense);
            StatusClass = "alert-success";
            Message = "Recurring expense updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }

    protected void DefaultSelected(int id) {
        RecurringExpense.RecurringExpenseDefaultId = id;
        var defAmount = RecurringExpenseDefaults.Where(r => r.Id == id).Select(r => r.DefaultAmount).Single();
        if (defAmount != null)
            RecurringExpense.Amount = defAmount.Value;
    }
}