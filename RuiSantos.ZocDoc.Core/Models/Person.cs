namespace RuiSantos.ZocDoc.Core.Models;

public class Person
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<string> ContactNumbers { get; set; }

    public Person()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        ContactNumbers = Enumerable.Empty<string>();
    }

    public Person(string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ContactNumbers = contactNumbers;
    }
}

