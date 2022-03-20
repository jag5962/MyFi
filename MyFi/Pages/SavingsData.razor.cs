using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data.Models;
using MyFi.Data.Services;
using MyFi.Shared;

namespace MyFi.Pages;
public partial class SavingsData
{
    [Inject]
    private ISavingsDefaultService? DefaultService { get; set; }
    [Inject]
    private ISavingsService? SavingsService { get; set; }
    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<Savings>? savings;
    private IEnumerable<SavingsDefault>? savingsDefaults;
    private Confirm? DeleteConfirmation { get; set; }
    private Savings? savingsTarget;
    private SavingsDefault? defaultTarget;
    private bool deletingSavings;
    private string? deletionDescription;
    private ConfirmResult? DeleteConfirmResult { get; set; }
    private bool deleteIsSuccess;

    protected override async Task OnInitializedAsync() {
        savingsDefaults = await DefaultService.GetUserSavingsDefaultsAsync(HttpContextAccessor.HttpContext.User.Identity.Name);
        await AutoAddDefaults();
        await Month_Selected(new MonthSelect.SelectedMonth());
    }

    protected async Task Month_Selected(MonthSelect.SelectedMonth selected) {
        savings = await SavingsService.GetUserSavingsByMonthAsync(selected.StartOfMonth,
            HttpContextAccessor.HttpContext.User.Identity.Name);
    }

    protected void DeleteSavings_Click(Savings savings) {
        deletingSavings = true;
        savingsTarget = savings;
        deletionDescription = $"Savings {savings.Default.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected void DeleteDefault_Click(SavingsDefault savingsDefault) {
        deletingSavings = false;
        defaultTarget = savingsDefault;
        deletionDescription = $"Savings default {savingsDefault.Description}";
        DeleteConfirmation.ConfirmationMessage = "Are you sure you want to delete " +
                $"{deletionDescription}?";
        DeleteConfirmation.Show();
    }

    protected async Task ConfirmDelete_Click(bool confirmedDelete) {
        if (confirmedDelete) {
            deleteIsSuccess = deletingSavings ? await SavingsService.DeleteSavingsAsync(savingsTarget)
                : await DefaultService.DeleteSavingsDefaultAsync(defaultTarget);

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

    protected void ConfirmResult_Click() {
        if (deleteIsSuccess)
            NavigationManager.NavigateTo("savings", true);
    }

    private async Task AutoAddDefaults() {
        var today = DateTime.Today;
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        foreach (var def in savingsDefaults) {
            if (def.LatestAutoAddedMonth < startOfMonth && def.DefaultAmount != null) {
                await SavingsService.AddSavingsAsync(new Savings(def, startOfMonth),
                    HttpContextAccessor.HttpContext.User.Identity.Name);
                def.LatestAutoAddedMonth = startOfMonth;
                await DefaultService.UpdateSavingsDefaultAsync(def);
            }
        }
    }
}