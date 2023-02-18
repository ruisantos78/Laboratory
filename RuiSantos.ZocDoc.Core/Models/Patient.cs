using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Patient: Person
{
    public string? Id { get; set; }
    public string SocialSecurityNumber { get; set; }

	public Patient()
	{
		this.SocialSecurityNumber = string.Empty;
	}
}

