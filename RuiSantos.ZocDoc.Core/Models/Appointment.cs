using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Appointment
{
	public string? Id { get; set; }
	public DateTime Date { get; set; }
	public Patient Patient { get; set; }

	public Appointment()
	{
		this.Date = DateTime.MinValue;
		this.Patient = new Patient();
	}
}

