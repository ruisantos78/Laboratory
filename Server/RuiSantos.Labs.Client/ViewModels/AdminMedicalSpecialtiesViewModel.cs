using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;

namespace RuiSantos.Labs.Client.ViewModels;

[RegisterService]
public partial class AdminMedicalSpecialtiesViewModel: ObservableObject
{
    private readonly ILabsClient client;
    private readonly NavigationManager navigationManager;

    [ObservableProperty]
    private ObservableCollection<string> _specialties;

    public AdminMedicalSpecialtiesViewModel(
        ILabsClient client,
        NavigationManager navigationManager
    )
    {
        this.client = client;
        this.navigationManager = navigationManager;

        _specialties = new();
    }

    [RelayCommand]
    public void Add() {
        navigationManager.NavigateTo("/admin/specialties/add");    
    }    

    [RelayCommand]
    public void Remove(string specialty) {
        Specialties.Remove(specialty);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default) {
        Specialties.Clear();
        
        var response = await client.GetMedicalSpecialties.ExecuteAsync(cancellationToken);
        if (response.Data is null)
            return;
        
        response.Data.Specialties
            .ToList()
            .ForEach(x => Specialties.Add(x.Description));
    }     
}
