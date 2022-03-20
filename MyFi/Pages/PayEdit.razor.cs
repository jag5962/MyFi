using System;
using Microsoft.AspNetCore.Components;
using MyFi.Data;
using MyFi.Data.Models;
using MyFi.Data.Services;

namespace MyFi.Pages;
public partial class PayEdit
{
    [Inject]
    public IPayService PayService { get; set; } = default!;
    [Inject]
    public IIncomeService IncomeService { get; set; } = default!;
    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    public Pay Pay { get; set; } = new Pay(DateOnly.FromDateTime(DateTime.Now));
    public IEnumerable<Income> Incomes { get; set; } = new List<Income>();

    protected string Message = string.Empty;
    protected string StatusClass = string.Empty;
    protected bool Saved;

    protected override async Task OnInitializedAsync() {
        Saved = false;
        var username = HttpContextAccessor.HttpContext.User.Identity.Name;
        Incomes = await IncomeService.GetUserIncomesAsync(username);

        if (Id != 0)
            Pay = await PayService.GetPayByIdAsync(Id);

        if (Incomes.Count() == 1)
            DefaultSelected(Incomes.Select(i => i.Id).Single());
    }

    protected async Task HandleValidSubmit() {
        Saved = false;
        if (Pay.Id == 0) {
            bool successful = await PayService.AddPayAsync(Pay,
                HttpContextAccessor.HttpContext.User.Identity.Name);
            if (successful) {
                StatusClass = "alert-success";
                Message = "New pay added successfully.";
            } else {
                StatusClass = "alert-danger";
                Message = "An error has occurred when adding a new pay. Please try again.";
            }
        } else {
            await PayService.UpdatePayAsync(Pay);
            StatusClass = "alert-success";
            Message = "Pay updated successfully.";
        }
        Saved = true;
    }

    protected void HandleInvalidSubmit() {
        StatusClass = "alert-danger";
        Message = "Some entry is invalid. Please try again.";
    }

    protected void DefaultSelected(int id) {
        Pay.IncomeId = id;
        var defAmount = Incomes.Where(s => s.Id == id).Select(s => s.DefaultPay).Single();
        if (defAmount != null)
            Pay.Amount = defAmount.Value;
    }
}