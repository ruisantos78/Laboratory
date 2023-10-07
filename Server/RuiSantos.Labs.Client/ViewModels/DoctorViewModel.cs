using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Blazing.Mvvm.ComponentModel;
using Blazorise;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using StrawberryShake;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class DoctorViewModel : ValidatorViewModelBase
{
	private readonly ILabsClient _client;
    private readonly NavigationManager _navigationManager;
    private readonly ILoadingIndicatorService _loadingIndicatorService;
    private readonly ILogger<DoctorViewModel> _logger;

	[Required]
	[ObservableProperty] private string _license = string.Empty;

	[Required]
	[ObservableProperty] private string _firstName = string.Empty;

	[Required]
	[ObservableProperty] private string _lastName = string.Empty;

	[Required, EmailAddress]
	[ObservableProperty] private string _email = string.Empty;

	[ObservableProperty] private List<string> _specialties = new();
	[ObservableProperty] private List<string> _contacts = new();
	
	[ObservableProperty] private string _contactSelected = string.Empty;
	[ObservableProperty] private string _specialtySelected = string.Empty;

	[ObservableProperty] private bool _loaded = false;
	[ObservableProperty] private bool _editing = false;

	public ObservableCollection<string> SpecialtiesOptions { get; } = new();

    public DoctorViewModel(
		ILabsClient client,
		NavigationManager navigationManager,
		ILoadingIndicatorService loadingIndicatorService,
		ILogger<DoctorViewModel> logger
	)
	{
		_client = client;
        _navigationManager = navigationManager;
        _loadingIndicatorService = loadingIndicatorService;
        _logger = logger;
    }

	[RelayCommand]
	public Task AddContact()
	{
		if (string.IsNullOrWhiteSpace(ContactSelected))
			return Task.CompletedTask;

		Contacts.Insert(0, ContactSelected.Trim());
		ContactSelected = string.Empty;
		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task RemoveContact(string contact)
	{
		Contacts.Remove(contact);
		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task AddSpecialty()
	{
		if (string.IsNullOrWhiteSpace(SpecialtySelected))
			return Task.CompletedTask;

		Specialties.Insert(0, SpecialtySelected.Trim());
		SpecialtiesOptions.Remove(SpecialtySelected);
		SpecialtySelected = string.Empty;

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task RemoveSpecialty(string specialty)
	{
		Specialties.Remove(specialty);
		SpecialtiesOptions.Add(specialty);
		SpecialtiesOptions.RemoveAt(SpecialtiesOptions.IndexOf(specialty));

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public async Task Store(Validations? validations)
	{
		await _loadingIndicatorService.Show();
		try
		{
			await AddContact();
			await AddSpecialty();

			if (validations is not null && await validations.ValidateAll() is false)
				return;

			var operationResult = await _client.SetDoctor.ExecuteAsync(new SetDoctorInput
            {
				Doctor = new() {
					License = License,
					FirstName = FirstName,
					LastName = LastName,
					Email = Email,
					Contacts = Contacts,
					Specialties = Specialties
				}
			});

			operationResult.EnsureNoErrors();			
		}
		catch(Exception ex)
		{
			_logger.LogCritical(ex, "Failure to store the doctor!");
			return;
		}
		finally
		{
			await _loadingIndicatorService.Hide();
		}

        _navigationManager.NavigateTo("/doctors");
    }

	public void ValidateSpecialties(ValidatorEventArgs e) {
		e.ErrorText = "The doctor must have at least one specialty";
		e.Status = Specialties.Count > 0 ? ValidationStatus.Success : ValidationStatus.Error;
	}

	public async Task InitializeAsync(string? id)
	{
		await _loadingIndicatorService.Show();
		try
		{
			await InitializeMedicalSpecialtiesOptionsAsync();

			Loaded = await TryGetDoctorAsync(id);			
		}
		finally
		{
			await _loadingIndicatorService.Hide();
		}
	}

    private async Task InitializeMedicalSpecialtiesOptionsAsync()
    {
        var response = await _client.GetMedicalSpecialties.ExecuteAsync();
        response.Data?.Specialties
            .Select(x => x.Description)
            .ToList()
            .ForEach(SpecialtiesOptions.Add);
    }

	private async Task<bool> TryGetDoctorAsync(string? id)
	{
        if (string.IsNullOrWhiteSpace(id))
            return true;

        var response = await _client.GetDoctor.ExecuteAsync(id);
		if (response.Data?.Doctor is not {} doctor)
		{
            _navigationManager.NavigateTo("/doctors");
            return false;
        }
			
		License = doctor.License;
		FirstName = doctor.FirstName;
		LastName = doctor.LastName;
		Email = doctor.Email;
		Contacts = doctor.Contacts.ToList();
		Specialties = doctor.Specialties.ToList();
		Specialties.ForEach(x => SpecialtiesOptions.Remove(x));

		Editing = true;
		return true;
	}
}