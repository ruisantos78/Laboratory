using Blazing.Mvvm.Components;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.ViewModels;

namespace RuiSantos.Labs.Client.Pages.Doctors;

public partial class Doctor : MvvmComponentBase<DoctorViewModel>
{
    Validations? validations;

    [Parameter] public string License { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (ViewModel is not null)
            await ViewModel.InitializeAsync(License);
    }

    public Visibility FormVisibilty => ViewModel?.Loaded is true
        ? Visibility.Visible
        : Visibility.Invisible;

    public IFluentDisplay RemoveButtonDisplay => ViewModel?.Editing is true
        ? Display.Block
        : Display.None;    
}