using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Schedule
{
    public string? Id { get; set; }
    public Doctor Doctor { get; set; }
	public IList<Availability> Availability { get; set; }
    public IList<Appointment> Appointments { get; set; }

    public Schedule()
	{
		this.Doctor = new Doctor();
		this.Availability = new List<Availability>();
        this.Appointments = new List<Appointment>();
    }
}