using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.ViewModels;

namespace RuiSantos.Labs.Client.Pages.Doctors
{
    public partial class Doctor : MvvmComponentBase<DoctorViewModel>
    {
        [Parameter] public string? Id { get; set; }
    }
}