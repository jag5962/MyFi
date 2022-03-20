using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Models;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class RecurringExpenseData
{
    [Inject]
    private IRecurringExpenseDefaultService? DefaultService { get; set; }
    [Inject]
    private IRecurringExpenseService? RecurringExpenseService { get; set; }
    [Inject]
    private IIncomeService? IncomeService { get; set; }
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<RecurringExpense>? recurringExpenses;
    private IEnumerable<RecurringExpenseDefault>? recurringExpenseDefaults;
    private double splitPercentage;
    private Confirm? DeleteConfirmation { get; set; }
    private RecurringExpense? recurringExpenseTarget;
    private RecurringExpenseDefault? defaultTarget;
    private bool deletingRecurringExpense;
    private string? deletionDescription;
    private ConfirmResult? DeleteConfirmResult { get; set; }
    private bool deleteIsSuccess;

    protected override async Task OnInitializedAsync() {
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        splitPercentage = await IncomeService.GetUserSplitPercent(username);
        recurringExpenseDefaults = await DefaultService.GetUserRecurringExpenseDefaultsAsync(username);
        await AutoAddDefaults();
        await Month_Selected(new MonthSelect.SelectedMonth());
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        recurringExpenses = await RecurringExpenseService.GetUserRecurringExpenseByMonthAsync(selected.StartOfMonth,
            HttpContextAccessor.HttpContext.User.Identity.Name);
    }

    protected void DeleteRecurringExpense_Click(RecurringExpense recurringExpense) {
        deletingRecurringExpense = true;
        recurringExpenseTarget = recurringExpense;
        deletionDescription = $"Recurring expense {recurringExpense.Default.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected void DeleteDefault_Click(RecurringExpenseDefault recurringExpenseDefault) {
        deletingRecurringExpense = false;
        defaultTarget = recurringExpenseDefault;
        deletionDescription = $"Recurring expense default {recurringExpenseDefault.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected async Task ConfirmDelete_Click(bool confirmedDelete) {
        if (confirmedDelete) {
            deleteIsSuccess = deletingRecurringExpense ? await RecurringExpenseService.DeleteRecurringExpenseAsync(recurringExpenseTarget)
                : await DefaultService.DeleteRecurringExpenseDefaultAsync(defaultTarget);

            if (deleteIsSuccess) {
                DeleteConfirmResult.ConfirmationTitle = "Success";
                DeleteConfirmResult.ConfirmationMessage = $"{deletionDescription} was successfully deleted!";
            } else {
                DeleteConfirmResult.ConfirmationTitle = "Error";
                DeleteConfirmResult.ConfirmationMessage = $"{deletionDescription} has failed to delete!";
            }
            DeleteConfirmResult.Show();
        }
    }

    protected async void ConfirmResult_Click() {
        if (deleteIsSuccess)
            NavigationManager.NavigateTo("expenses/recurring", true);
    }

    private async Task AutoAddDefaults() {
        var today = DateTime.Today;
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        foreach (var def in recurringExpenseDefaults) {
            if (def.LatestAutoAddedMonth < startOfMonth && def.DefaultAmount != null) {
                await RecurringExpenseService.AddRecurringExpenseAsync(new RecurringExpense(def, startOfMonth),
                    HttpContextAccessor.HttpContext.User.Identity.Name);
                def.LatestAutoAddedMonth = startOfMonth;
                await DefaultService.UpdateRecurringExpenseDefaultAsync(def);
            }
        }
    }
}