using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Models;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class IncomeData
{
    [Inject]
    private IIncomeService? IncomeService { get; set; }
    [Inject]
    private IPayService? PayService { get; set; }
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<Income>? incomes;
    private IEnumerable<Pay>? pays;
    private Confirm? DeleteConfirmation { get; set; }
    private Pay? targetPay;
    private Income? targetIncome;
    private bool deletingPay;
    private string? deletionDescription;
    private ConfirmResult? DeleteConfirmResult { get; set; }
    private bool deleteIsSuccess;

    protected override async Task OnInitializedAsync() {
        incomes = await IncomeService.GetUserIncomesAsync(HttpContextAccessor.HttpContext.User.Identity.Name);
        await AutoAddDefaults();
        await Month_Selected(new MonthSelect.SelectedMonth());
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        pays = await PayService.GetUserPaysByDateAsync(selected.StartOfMonth, selected.EndOfMonth,
            HttpContextAccessor.HttpContext.User.Identity.Name);
    }

    protected void DeletePay_Click(Pay pay) {
        deletingPay = true;
        targetPay = pay;
        deletionDescription = $"{pay.IncomeSource.Description} {pay.Date}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected void DeleteIncome_Click(Income income) {
        deletingPay = false;
        targetIncome = income;
        deletionDescription = income.Description;
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected async Task ConfirmDelete_Click(bool confirmedDelete) {
        if (confirmedDelete) {
            deleteIsSuccess = deletingPay ? await PayService.DeletePayAsync(targetPay)
                : await IncomeService.DeleteIncomeAsync(targetIncome);

            if (deleteIsSuccess) {
                DeleteConfirmResult.ConfirmationTitle = "Success";
                DeleteConfirmResult.ConfirmationMessage = (deletingPay ? "Pay" : "Income source") +
                    $" {deletionDescription} was successfully deleted!";
            } else {
                DeleteConfirmResult.ConfirmationTitle = "Error";
                DeleteConfirmResult.ConfirmationMessage = (deletingPay ? "Pay" : "Income source") +
                $" {deletionDescription} has failed to delete!";
            }
            DeleteConfirmResult.Show();
        }
    }

    protected void ConfirmResult_Click() {
        if (deleteIsSuccess)
            NavigationManager.NavigateTo("income", true);
    }

    private async Task AutoAddDefaults() {
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        var today = DateTime.Today;
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        var endOfMonth = new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

        foreach (var def in incomes) {
            if (def.Type.Id == 1 && def.LatestAutoAddedMonth < startOfMonth && def.DefaultPay != null) {
                var latestPayDate = await PayService.GetUserLatestPayDate(def, username);
                latestPayDate = latestPayDate.AddDays(14);

                while (latestPayDate <= endOfMonth) {
                    await PayService.AddPayAsync(new Pay(def, latestPayDate), username);
                    latestPayDate = latestPayDate.AddDays(14);
                }

                def.LatestAutoAddedMonth = startOfMonth;
                await IncomeService.UpdateIncomeAsync(def);
            }
        }
    }
}