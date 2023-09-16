using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class IndexViewModel : ViewModelBase
{
    private readonly IProfilesOperations profilesOperations;
    private readonly NavigationManager navigationManager;

    [ObservableProperty] private string _profile = string.Empty ;
    [ObservableProperty] private string _operation = string.Empty;
    [ObservableProperty] private IReadOnlyCollection<string> _profiles = Array.Empty<string>();
    [ObservableProperty] private IReadOnlyCollection<Operation> _operations = Array.Empty<Operation>();
    [ObservableProperty] private bool _disableConfirmButton = true;

    public IndexViewModel(
        IProfilesOperations profilesOperations,
        NavigationManager navigationManager
    )
    {
        this.profilesOperations = profilesOperations;
        this.navigationManager = navigationManager;
    }

    [RelayCommand]
    public Task Confirm()
    {
        if (Guid.TryParse(Operation, out var operation))
            navigationManager.NavigateTo(profilesOperations.GetRoute(operation));

        return Task.CompletedTask;
    }

    partial void OnProfileChanged(string value)
    {
        this.Operation = string.Empty;
        this.Operations = profilesOperations.GetOperations(value);
    }

    partial void OnOperationChanged(string value)
    {
        this.DisableConfirmButton = string.IsNullOrEmpty(value);
    }

    public override Task Loaded()
    {
        Profiles = profilesOperations.GetAllProfiles();
        return Task.CompletedTask;
    }
}
