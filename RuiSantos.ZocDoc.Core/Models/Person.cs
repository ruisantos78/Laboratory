namespace RuiSantos.ZocDoc.Core.Models;

public class Person
{
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public IEnumerable<string> ContactNumbers { get; init; }

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

