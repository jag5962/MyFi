using System;
using Microsoft.AspNetCore.Components;

namespace MyFi.Shared;
public partial class ConfirmResult : ComponentBase
{
    protected bool ShowConfirmationResult { get; set; }

    [Parameter]
    public string ConfirmationTitle { get; set; } = "Deletion Result";

    [Parameter]
    public string ConfirmationMessage { get; set; } = string.Empty;

    public void Show() {
        ShowConfirmationResult = true;
        StateHasChanged();
    }

    [Parameter]
    public EventCallback<bool> OnClosed { get; set; }

    protected async Task CloseModal() {
        ShowConfirmationResult = false;
        await OnClosed.InvokeAsync();
    }
}