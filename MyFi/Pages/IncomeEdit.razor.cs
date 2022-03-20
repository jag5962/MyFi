using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class IncomeEdit
{
    [Inject]
    private IIncomeService IncomeService { get; set; } = default!;
    [Inject]
    private IPayService PayService { get; set; } = default!;
    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private Income Income { get; set; } = new Income();
    private IEnumerable<IncomeType> IncomeTypes { get; set; } = new List<IncomeType>();
    private DateOnly? FirstPayDate { get; set; }

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        IncomeTypes = await IncomeService.GetIncomeTypesAsync();

        Income = Id == 0 ? new Income() : await IncomeService.GetIncomeByIdAsync(Id);
    }

    protected async Task HandleValidSubmit() {
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        Saved = false;
        if (Income.IncomeTypeId == 1 && Income.AnnualGrossPay == 0) {
            StatusClass = "alert-danger";
            Message = "There must be an annual gross pay for primary incomes. Please try again.";
        } else if (Income.Id == 0) {
            if (Income.IncomeTypeId == 1 && Income.DefaultPay != null &&
                (FirstPayDate == null || FirstPayDate == DateOnly.MinValue)) {
                StatusClass = "alert-danger";
                Message = "Please enter the first pay check date when adding a new primary income. Please try again.";
                Saved = true;
                return;
            }
            bool successful = await IncomeService.AddIncomeAsync(Income, username);
            if (successful) {
                if (Income.IncomeTypeId == 1 && Income.DefaultPay != null) {
                    bool payAdded = await PayService.AddPayAsync(new Pay(Income, FirstPayDate.Value), username);
                    if (!payAdded) {
                        await IncomeService.DeleteIncomeAsync(Income);
                        StatusClass = "alert-danger";
                        Message = "An error has occurred when adding a new pay after adding the income. Please try again.";
                        Saved = true;
                        return;
                    }
                }
                StatusClass = "alert-success";
                Message = "New income added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new income. Please try again.";
            }
        } else {
            await IncomeService.UpdateIncomeAsync(Income);
            StatusClass = "alert-success";
            Message = "Income updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }
}