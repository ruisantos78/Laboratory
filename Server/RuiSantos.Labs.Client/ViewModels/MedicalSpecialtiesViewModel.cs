using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RuiSantos.Labs.Client.Core;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class MedicalSpecialtiesViewModel : ViewModelBase
{
    private readonly IMedicalSpecialtiesService _medicalSpecialtiesService;
    private readonly ILoadingIndicatorService _loadingIndicatorService;
    private readonly ILogger<MedicalSpecialtiesViewModel> _logger;

    [ObservableProperty] ObservableCollection<string> _specialties = new();
    [ObservableProperty] bool _modalVisible;
    [ObservableProperty] string _inputSpecialty = string.Empty;
    [ObservableProperty] ObservableCollection<string> _inputSpecialties = new();

    public MedicalSpecialtiesViewModel(
        IMedicalSpecialtiesService medicalSpecialtiesService,
        ILoadingIndicatorService loadingIndicatorService,
        ILogger<MedicalSpecialtiesViewModel> loggeer
    )
    {
        _medicalSpecialtiesService = medicalSpecialtiesService;
        _loadingIndicatorService = loadingIndicatorService;
        _logger = loggeer;
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

        var specialties = await _medicalSpecialtiesService.StoreMedicalSpecialtiesAsync(InputSpecialties);
        specialties.ForEach(InsertSpecialtyOnList);

        await CloseModal();

        void InsertSpecialtyOnList(string value)
        {
            var index = Specialties.IndexOf(Specialties
                .FirstOrDefault(x => String
                    .Compare(x, value, StringComparison.Ordinal) > 0, string.Empty));

            if (index < 0)
                Specialties.Add(value);
            else
                Specialties.Insert(index, value);
        }
    }

    [RelayCommand]
    public async Task Remove(string specialty)
    {
        var specialties = await _medicalSpecialtiesService.RemoveMedicalSpecialtyAsync(specialty);
        specialties.ForEach(x => Specialties.Remove(x));
    }

    public void SetInputSpecialty(string value)
    {
        var values = value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Array.ForEach(values, x => InputSpecialties.Add(x));

        InputSpecialty = string.Empty;
    }

    public override async Task Loaded()
    {
        await _loadingIndicatorService.Show();
        try
        {
            Specialties.Clear();

            var specialties = await _medicalSpecialtiesService.GetMedicalSpecialtiesAsync();
            specialties.ForEach(x => Specialties.Add(x));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error loading medical specialties!");
        }
        finally
        {
            await _loadingIndicatorService.Hide();
        }
    }
}