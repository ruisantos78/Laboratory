using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Patient : Person
{
    public Guid Id { get; set; }
    public string SocialSecurityNumber { get; set; }

    public Patient()
    {
        this.Id = Guid.NewGuid();
        this.SocialSecurityNumber = string.Empty;
    }
}

