using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Models;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class ExpenseData
{
    [Inject]
    private IExpenseService? ExpenseService { get; set; }
    [Inject]
    private IIncomeService? IncomeService { get; set; }
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    [Parameter]
    public string Type { get; set; } = default!;

    private IEnumerable<Expense>? expenses;
    private double splitPercentage;
    private Confirm? DeleteConfirmation { get; set; }
    private Expense? targetExpense;
    private string? deletionDescription;
    private ConfirmResult? DeleteConfirmResult { get; set; }
    private bool deleteIsSuccess;

    protected override async Task OnParametersSetAsync() {
        if (Type == "shared")
            splitPercentage = await IncomeService.GetUserSplitPercent(HttpContextAccessor.HttpContext.User.Identity.Name);
        else
            splitPercentage = 1;

        await Month_Selected(new MonthSelect.SelectedMonth());
        base.OnParametersSet();
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        if (Type == "personal")
            expenses = await ExpenseService.GetUserExpensesByDateAsync(selected.StartOfMonth, selected.EndOfMonth,
                HttpContextAccessor.HttpContext.User.Identity.Name);
        else if (Type == "shared")
            expenses = await ExpenseService.GetSharedExpensesByDateAsync(selected.StartOfMonth, selected.EndOfMonth);
    }

    protected void DeleteExpense_Click(Expense expense) {
        targetExpense = expense;
        deletionDescription = $"{expense.Description} {expense.Date}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected async Task ConfirmDelete_Click(bool confirmedDelete) {
        if (confirmedDelete) {
            deleteIsSuccess = await ExpenseService.DeleteExpenseAsync(targetExpense);

            if (deleteIsSuccess) {
                DeleteConfirmResult.ConfirmationTitle = "Success";
                DeleteConfirmResult.ConfirmationMessage = $"Expense {deletionDescription} was successfully deleted!";
            } else {
                DeleteConfirmResult.ConfirmationTitle = "Error";
                DeleteConfirmResult.ConfirmationMessage = $"Expense {deletionDescription} has failed to delete!";
            }
            DeleteConfirmResult.Show();
        }
    }

    protected void ConfirmResult_Click() {
        if (deleteIsSuccess)
            NavigationManager.NavigateTo($"expenses/{Type}", true);
    }
}