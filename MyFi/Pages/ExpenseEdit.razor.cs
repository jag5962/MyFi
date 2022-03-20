using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class ExpenseEdit
{
    [Inject]
    public IExpenseService ExpenseService { get; set; } = default!;
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public string Type { get; set; } = default!;
    [Parameter]
    public int Id { get; set; }

    public Expense Expense { get; set; } = new Expense(DateOnly.FromDateTime(DateTime.Now));
    public IEnumerable<ExpenseType> ExpenseTypes { get; set; } = new List<ExpenseType>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        ExpenseTypes = await ExpenseService.GetExpenseTypesAsync();

        if (Id != 0)
            Expense = await ExpenseService.GetExpenseByIdAsync(Id);

        Expense.Shared = Type == "shared";
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (Expense.Id == 0) {
            bool successful = await ExpenseService.AddExpenseAsync(Expense,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New expense added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new expense. Please try again.";
            }
        } else {
            await ExpenseService.UpdateExpenseAsync(Expense);
            StatusClass = "alert-success";
            Message = "Expense updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }
}