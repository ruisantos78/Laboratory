using Blazorise.DataGrid;
using RuiSantos.Labs.Client.Core;
using RuiSantos.Labs.Client.Models;

namespace RuiSantos.Labs.Client.Pages.Doctors;

partial class Index
{
    public bool NoSelectable(RowSelectableEventArgs<DoctorModel> e) => false;

    public readonly VirtualizeOptions VirtualizeOptions = new()
    {
        DataGridHeight = "auto",
        DataGridMaxHeight = "75vh"
    };

    public Func<DoctorModel?, string> LicenseCellClass => _ => Css.NoWrap;

    public string NewLink() => "/doctor";
    public string EditLink(string? license) => $"/doctor/{license}";
}
