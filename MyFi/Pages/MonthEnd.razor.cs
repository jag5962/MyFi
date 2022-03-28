using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Models;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class MonthEnd
{
    const string JOHN = "johnagilb@gmail.com";
    const string MARY = "mrlys_pagan@yahoo.com";

    [Inject]
    private IExpenseService? ExpenseService { get; set; }
    [Inject]
    private IRecurringExpenseService? RecurringExpenseService { get; set; }
    [Inject]
    private IIncomeService? IncomeService { get; set; }
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<RecurringExpense>? recurringExpenses;
    private IEnumerable<Expense>? sharedExpenses;
    private double splitPercentage;
    private Confirm? DeleteConfirmation { get; set; }
    private RecurringExpense? recurringExpenseTarget;
    private Expense? sharedExpenseTarget;
    private bool deletingRecurringExpense;
    private string? deletionDescription;
    private ConfirmResult? DeleteConfirmResult { get; set; }
    private bool deleteIsSuccess;
    private decimal? johnsTotal, marysTotal, total;
    private decimal? johnsSplit, marysSplit, owedToJohn;

    protected override async Task OnInitializedAsync() {
        await Month_Selected(new MonthSelect.SelectedMonth());
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        recurringExpenses = await RecurringExpenseService.GetAllSharedRecurringExpenseByMonthAsync(selected.StartOfMonth);
        sharedExpenses = await ExpenseService.GetSharedExpensesByDateAsync(selected.StartOfMonth, selected.EndOfMonth);
        await CalculateTotals();
    }

    protected void DeleteRecurringExpense_Click(RecurringExpense recurringExpense) {
        deletingRecurringExpense = true;
        recurringExpenseTarget = recurringExpense;
        deletionDescription = $"Recurring expense {recurringExpense.Default.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected void DeleteSharedExpense_Click(Expense sharedExpense) {
        deletingRecurringExpense = false;
        sharedExpenseTarget = sharedExpense;
        deletionDescription = $"Shared expense {sharedExpense.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected async Task ConfirmDelete_Click(bool confirmedDelete) {
        if (confirmedDelete) {
            deleteIsSuccess = deletingRecurringExpense ? await RecurringExpenseService.DeleteRecurringExpenseAsync(recurringExpenseTarget)
                : await ExpenseService.DeleteExpenseAsync(sharedExpenseTarget);

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
            NavigationManager.NavigateTo("monthend", true);
    }

    private async Task CalculateTotals() {
        johnsTotal = recurringExpenses.Where(r => r.Username == JOHN).Sum(r => r.Amount) +
                    sharedExpenses.Where(r => r.Username == JOHN).Sum(r => r.Amount);
        marysTotal = recurringExpenses.Where(r => r.Username == MARY).Sum(r => r.Amount) +
                    sharedExpenses.Where(r => r.Username == MARY).Sum(r => r.Amount);
        total = johnsTotal + marysTotal;

        johnsSplit = (decimal)((double)total * await IncomeService.GetUserSplitPercent(JOHN));
        marysSplit = total - johnsSplit;

        owedToJohn = marysSplit - marysTotal;
    }
}