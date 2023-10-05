using Blazing.Mvvm.Components;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.ViewModels;

namespace RuiSantos.Labs.Client.Pages.Doctors;

public partial class Doctor : MvvmComponentBase<DoctorViewModel>
{
    Validations? validations;

    [Parameter] public string? Id { get; set; }
}