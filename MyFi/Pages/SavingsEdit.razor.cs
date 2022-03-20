using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class SavingsEdit
{
    [Inject]
    public ISavingsService SavingsService { get; set; } = default!;
    [Inject]
    public ISavingsDefaultService DefaultService { get; set; } = default!;
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    public Savings Savings { get; set; } = new Savings(DateOnly.FromDateTime(
        new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)));
    public IEnumerable<SavingsDefault> SavingsDefaults { get; set; } = new List<SavingsDefault>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        SavingsDefaults = await DefaultService.GetUserSavingsDefaultsAsync(username);

        if (Id != 0)
            Savings = await SavingsService.GetSavingsByIdAsync(Id);

        if (SavingsDefaults.Count() == 1)
            DefaultSelected(SavingsDefaults.Select(s => s.Id).Single());
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (Savings.Id == 0) {
            bool successful = await SavingsService.AddSavingsAsync(Savings,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New savings added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new savings. Please try again.";
            }
        } else {
            await SavingsService.UpdateSavingsAsync(Savings);
            StatusClass = "alert-success";
            Message = "Savings updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }

    protected void DefaultSelected(int id) {
        Savings.SavingsDefaultId = id;
        var defAmount = SavingsDefaults.Where(s => s.Id == id).Select(s => s.DefaultAmount).Single();
        if (defAmount != null)
            Savings.Amount = defAmount.Value;
    }
}