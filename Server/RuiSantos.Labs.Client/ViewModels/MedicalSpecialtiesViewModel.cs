using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components.Web;
using StrawberryShake;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class MedicalSpecialtiesViewModel : ViewModelBase
{
    private readonly ILabsClient client;
    private readonly ILoadingIndicatorService loadingIndicatorService;
    private readonly ILogger<MedicalSpecialtiesViewModel> logger;
    [ObservableProperty] private ObservableCollection<string> _specialties = new();
    [ObservableProperty] private bool _modalVisible = false;
    [ObservableProperty] private string _inputSpecialty = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _inputSpecialties = new();

    public MedicalSpecialtiesViewModel(
        ILabsClient client,
        ILoadingIndicatorService loadingIndicatorService,
        ILogger<MedicalSpecialtiesViewModel> loggeer
    )
    {
        this.client = client;
        this.loadingIndicatorService = loadingIndicatorService;
        this.logger = loggeer;
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
            InputSpecialties.Insert(0, InputSpecialty.Trim());

        var response = await client.AddSpecialties.ExecuteAsync(new AddSpecialtiesInput()
        {
            Descriptions = InputSpecialties
        });

        if (response.IsSuccessResult() && response.Data?.AddSpecialties?.Specialties?.Any() is true)
        {
            foreach(var item in response.Data.AddSpecialties.Specialties) 
            {
                var index = Specialties.IndexOf(Specialties.FirstOrDefault(x => x.CompareTo(item.Description) > 0, string.Empty));
                if (index < 0) 
                    Specialties.Add(item.Description);
                else 
                    Specialties.Insert(index, item.Description);
            }
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
            response.Data.RemoveSpecialties.Specialties.ToList()
                .ForEach(x => Specialties.Remove(x.Description));            
        }
    }

    public Task OnInputSpecialtyKeyPress(KeyboardEventArgs e)
    {
        if (e.Key is not "Enter" || string.IsNullOrWhiteSpace(InputSpecialty))
            return Task.CompletedTask;

        InputSpecialty.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList()
            .ForEach(x => InputSpecialties.Insert(0, x));

        InputSpecialty = string.Empty;

        return Task.CompletedTask;
    }

    public override async Task Loaded()
    {
        await loadingIndicatorService.Show();
        try
        {
            Specialties.Clear();

            var operationResult = await client.GetMedicalSpecialties.ExecuteAsync();

            operationResult.EnsureNoErrors();

            operationResult.Data?.Specialties
                .ToList()
                .ForEach(x => Specialties.Add(x.Description));
        }
        catch(Exception ex)
        {
            logger.LogCritical(ex, "Error loading medical specialties!");
        }
        finally
        {
            await loadingIndicatorService.Hide();
        }
    }
}