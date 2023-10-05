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
	private readonly ILabsClient client;
    private readonly NavigationManager navigationManager;
    private readonly ILoadingIndicatorService loadingIndicatorService;
    private readonly ILogger<DoctorViewModel> logger;

	[Required]
    [ObservableProperty] public string _license = string.Empty;

	[Required]
	[ObservableProperty] public string _firstName = string.Empty;

	[Required]
	[ObservableProperty] public string _lastName = string.Empty;

	[Required, EmailAddress]
	[ObservableProperty] public string _email = string.Empty;

	[ObservableProperty] public List<string> _specialties = new();
	[ObservableProperty] public List<string> _contacts = new();
	
	[ObservableProperty] public string _contactSelected = string.Empty;
    [ObservableProperty] public string _specialtySelected = string.Empty;

	public ObservableCollection<string> SpecialtiesOptions { get; } = new();

    public DoctorViewModel(
		ILabsClient client,
		NavigationManager navigationManager,
		ILoadingIndicatorService loadingIndicatorService,
		ILogger<DoctorViewModel> logger
	)
	{
		this.client = client;
        this.navigationManager = navigationManager;
        this.loadingIndicatorService = loadingIndicatorService;
        this.logger = logger;
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
		await loadingIndicatorService.Show();
		try
		{
			await AddContact();
			await AddSpecialty();

			if (validations is not null && await validations.ValidateAll() is false)
				return;

			var operationResult = await client.SetDoctor.ExecuteAsync(new SetDoctorInput() {
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
			logger.LogCritical(ex, "Failure to store the doctor!");
			return;
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}

        navigationManager.NavigateTo("/doctors");
    }

	public void ValidateSpecialties(ValidatorEventArgs e) {
		e.ErrorText = "The doctor must have at least one specialty";
		e.Status = Specialties.Count > 0 ? ValidationStatus.Success : ValidationStatus.Error;
	}

	public override async Task Loaded()
	{
		await loadingIndicatorService.Show();
		try
        {
            await InitializeMedicalSpecialtiesOptionsAsync();
        }
        finally
		{
			await loadingIndicatorService.Hide();
		}
	}

    private async Task InitializeMedicalSpecialtiesOptionsAsync()
    {
        var response = await client.GetMedicalSpecialties.ExecuteAsync();
        response?.Data?.Specialties?
            .Select(x => x.Description)
            .ToList()
            .ForEach(SpecialtiesOptions.Add);
    }
}