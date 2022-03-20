using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class RecurringExpenseDefaultEdit
{
    [Inject]
    public IRecurringExpenseDefaultService DefaultService { get; set; } = default!;

    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    public RecurringExpenseDefault RecurringExpenseDefault { get; set; } = new RecurringExpenseDefault();
    public IEnumerable<RecurringExpenseType> RecurringExpenseTypes { get; set; } = new List<RecurringExpenseType>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        RecurringExpenseTypes = await DefaultService.GetRecurringExpenseTypesAsync();
        RecurringExpenseDefault = Id == 0 ? new RecurringExpenseDefault() : await DefaultService.GetRecurringExpenseDefaultByIdAsync(Id);
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (RecurringExpenseDefault.Id == 0) {
            bool successful = await DefaultService.AddRecurringExpenseDefaultAsync(RecurringExpenseDefault,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New recurring expense default added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new recurring expense default. Please try again.";
            }
        } else {
            await DefaultService.UpdateRecurringExpenseDefaultAsync(RecurringExpenseDefault);
            StatusClass = "alert-success";
            Message = "Recurring expense default updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }
}