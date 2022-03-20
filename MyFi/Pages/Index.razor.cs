using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class Index
{
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private IPayService? PayService { get; set; }
    [Inject]
    private IRecurringExpenseService? RecurringExpenseService { get; set; }
    [Inject]
    private ISavingsService? SavingsService { get; set; }
    [Inject]
    private IExpenseService? ExpenseService { get; set; }
    [Inject]
    private IIncomeService? IncomeService { get; set; }

    private decimal MonthlyIncome;
    private decimal RecurringExpenses;
    private decimal MonthlySavings;
    private decimal PersonalExpenses;
    private decimal SharedExpenses;

    protected override async Task OnInitializedAsync() {
        if (HttpContextAccessor.HttpContext.User.Identities.Count() > 0)
            await Month_Selected(new MonthSelect.SelectedMonth());
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        var split = await IncomeService.GetUserSplitPercent(username);

        MonthlyIncome = await PayService.GetUserMonthlyIncome(selected.StartOfMonth, selected.EndOfMonth, username);

        RecurringExpenses = await RecurringExpenseService.GetUserMonthlyRecurringExpenses(selected.StartOfMonth, username, split);

        MonthlySavings = await SavingsService.GetUserMonthlySavings(selected.StartOfMonth, username);

        PersonalExpenses = await ExpenseService.GetUserMonthlyPersonalExpenses(selected.StartOfMonth, selected.EndOfMonth, username);

        SharedExpenses = await ExpenseService.GetMonthlySharedExpenses(selected.StartOfMonth, selected.EndOfMonth, split);
    }
}