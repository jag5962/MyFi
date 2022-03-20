using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class SavingsDefaultEdit
{
    [Inject]
    public ISavingsDefaultService DefaultService { get; set; } = default!;

    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    public SavingsDefault SavingsDefault { get; set; } = new SavingsDefault();
    public IEnumerable<SavingsType> SavingsTypes { get; set; } = new List<SavingsType>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        SavingsTypes = await DefaultService.GetSavingsTypesAsync();
        SavingsDefault = Id == 0 ? new SavingsDefault() : await DefaultService.GetSavingsDefaultByIdAsync(Id);
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (SavingsDefault.Id == 0) {
            bool successful = await DefaultService.AddSavingsDefaultAsync(SavingsDefault,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New savings default added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new savings default. Please try again.";
            }
        } else {
            await DefaultService.UpdateSavingsDefaultAsync(SavingsDefault);
            StatusClass = "alert-success";
            Message = "Savings default updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }
}