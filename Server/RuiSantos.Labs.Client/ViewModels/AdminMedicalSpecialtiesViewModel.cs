using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components.Web;
using StrawberryShake;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class AdminMedicalSpecialtiesViewModel : ViewModelBase
{
    private readonly ILabsClient client;
    private readonly ILoadingIndicatorService loadingIndicatorService;

    [ObservableProperty] private LinkedList<string> _specialties = new();
    [ObservableProperty] private bool _modalVisible = false;
    [ObservableProperty] private string _inputSpecialty = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _inputSpecialties = new();

    public AdminMedicalSpecialtiesViewModel(
        ILabsClient client,
        ILoadingIndicatorService loadingIndicatorService)
    {
        this.client = client;
        this.loadingIndicatorService = loadingIndicatorService;
    }

    [RelayCommand]
    public Task AddNew()
    {
        InputSpecialty = string.Empty;
        InputSpecialties.Clear();

        ModalVisible = true;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task RemoveSpecialtiesInput(string specialty)
    {
        InputSpecialties.Remove(specialty);
        NotifyStateChanged();
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task CloseModal()
    {
        ModalVisible = false;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task Save()
    {
        if (!string.IsNullOrWhiteSpace(InputSpecialty))
            InputSpecialties.Add(InputSpecialty.Trim());

        var response = await client.AddSpecialties.ExecuteAsync(new AddSpecialtiesInput()
        {
            Descriptions = InputSpecialties
        });

        if (response.IsSuccessResult() && response.Data?.AddSpecialties?.Specialties?.Any() is true)
        {
            // Append each new specialty to the list in alphabetical order
            foreach (var item in response.Data.AddSpecialties.Specialties.Select(x => x.Description))
            {
                var index = Specialties.LastOrDefault(x => x.CompareTo(item) < 0, string.Empty);
                if (Specialties.Find(index) is { } node)
                    Specialties.AddAfter(node, item);
                else
                    Specialties.AddFirst(item);
            }

            NotifyStateChanged();
        }

        await CloseModal();
    }

    [RelayCommand]
    public async Task Remove(string specialty)
    {
        var response = await client.RemoveSpecialties.ExecuteAsync(new RemoveSpecialtiesInput()
        {
            Description = specialty
        });

        if (response.IsSuccessResult() && response.Data?.RemoveSpecialties?.Specialties?.Any() is true)
        {
            response.Data.RemoveSpecialties.Specialties
                .ToList()
                .ForEach(x => Specialties.Remove(x.Description));

            NotifyStateChanged();
        }
    }

    public Task OnInputSpecialtyKeyPress(KeyboardEventArgs e)
    {
        if (e.Key is not "Enter" || string.IsNullOrWhiteSpace(InputSpecialty))
            return Task.CompletedTask;

        InputSpecialty.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList()
            .ForEach(InputSpecialties.Add);

        InputSpecialty = string.Empty;

        return Task.CompletedTask;
    }

    public override async Task Loaded()
    {
        await loadingIndicatorService.Show();
        try
        {
            Specialties.Clear();

            var response = await client.GetMedicalSpecialties.ExecuteAsync();
            if (response.IsSuccessResult() && response.Data?.Specialties?.Any() is true)
            {
                Specialties = new(response.Data.Specialties
                    .OrderBy(x => x.Description)
                    .Select(x => x.Description));
            }

            NotifyStateChanged();
        }
        finally
        {
            await loadingIndicatorService.Hide();
        }
    }
}