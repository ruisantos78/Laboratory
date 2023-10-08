using Blazorise;
using Microsoft.AspNetCore.Components;

namespace RuiSantos.Labs.Client.Pages.Doctors;

partial class Doctor
{
    protected Validations? Validations;

    [Parameter] public string License { get; set; } = string.Empty;

    protected override Task OnInitializedAsync()
    {
        return ViewModel?.InitializeAsync(License) ?? Task.CompletedTask;
    }

    public Func<Task<bool>> IsValid => () => Validations?.ValidateAll() ?? Task.FromResult(false);  

    public Visibility FormVisibilty => ViewModel?.Visible is true
        ? Visibility.Visible
        : Visibility.Invisible;

    public IFluentDisplay RemoveButtonDisplay => ViewModel?.Editing is true
        ? Display.Block
        : Display.None;

    public void ValidateSpecialties(ValidatorEventArgs e)
    {
        e.ErrorText = "The doctor must have at least one specialty";
        e.Status = ViewModel?.Specialties.Count > 0 ? ValidationStatus.Success : ValidationStatus.Error;
    }
}