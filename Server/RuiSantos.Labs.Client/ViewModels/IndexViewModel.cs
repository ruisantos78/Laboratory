using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class IndexViewModel : ViewModelBase
{
    private readonly IProfilesOperations _profilesOperations;
    private readonly NavigationManager _navigationManager;

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
        _profilesOperations = profilesOperations;
        _navigationManager = navigationManager;
    }

    [RelayCommand]
    public Task Confirm()
    {
        if (Guid.TryParse(Operation, out var operation))
            _navigationManager.NavigateTo(_profilesOperations.GetRoute(operation));

        return Task.CompletedTask;
    }

    partial void OnProfileChanged(string value)
    {
        Operation = string.Empty;
        Operations = _profilesOperations.GetOperations(value);
    }

    partial void OnOperationChanged(string value)
    {
        DisableConfirmButton = string.IsNullOrEmpty(value);
    }

    public override Task Loaded()
    {
        Profiles = _profilesOperations.GetAllProfiles();
        return Task.CompletedTask;
    }
}
