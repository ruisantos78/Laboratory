using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Schedule
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public IDictionary<string, TimeSpan[]> OfficeHours { get; set; }
    public List<Appointment> Appointments { get; set; }

    public Schedule()
    {
        this.Id = Guid.NewGuid();
        this.DoctorId = Guid.Empty;
        this.OfficeHours = new Dictionary<string, TimeSpan[]>();
        this.Appointments = new List<Appointment>();
    }
}