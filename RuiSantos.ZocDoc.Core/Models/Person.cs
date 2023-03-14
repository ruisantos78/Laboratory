namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// A person.
/// </summary>
public class Person
{
    /// <summary>
    /// The email address of the person.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// The first name of the person.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The last name of the person.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// The contact numbers of the person.
    /// </summary>
    public IEnumerable<string> ContactNumbers { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    public Person()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        ContactNumbers = Enumerable.Empty<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    /// <param name="email">The email address of the person.</param>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>    
    /// <param name="contactNumbers">The contact numbers of the person.</param>
    public Person(string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ContactNumbers = contactNumbers;
    }
}

