using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

[RegisterService]
public partial class IndexViewModel : ObservableObject
{
    private readonly IProfilesOperations profilesOperations;
    private readonly NavigationManager navigationManager;

    [ObservableProperty]
    private string _profile;

    [ObservableProperty]
    private string _operation;

    [ObservableProperty]
    private IReadOnlyCollection<string> _profiles;

    [ObservableProperty]
    private IReadOnlyCollection<Operation> _operations;

    [ObservableProperty]
    private bool _disableConfirmButton;

    public IndexViewModel(
        IProfilesOperations profilesOperations,
        NavigationManager navigationManager
    )
    {
        this.profilesOperations = profilesOperations;
        this.navigationManager = navigationManager;

        _profile = string.Empty;
        _operation = string.Empty;
        _profiles = this.profilesOperations.GetAllProfiles();
        _operations = Array.Empty<Operation>();
        _disableConfirmButton = true;
    }

    [RelayCommand]
    public void Confirm()
    {
        if (Guid.TryParse(Operation, out var operation))
            navigationManager.NavigateTo(profilesOperations.GetRoute(operation));
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
}
