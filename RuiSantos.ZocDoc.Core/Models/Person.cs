using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Person
{
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public IList<string> ContactNumbers { get; set; }

	public Person()
	{
		this.FirstName = string.Empty;
		this.LastName = string.Empty;
		this.Email = string.Empty;
		this.ContactNumbers = new List<string>();
	}
}

