namespace RuiSantos.Labs.Core.Models;

/// <summary>
/// A person.
/// </summary>
public record Person
{
    /// <summary>
    /// The unique identifier of the persom.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The email address of the person.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The first name of the person.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the person.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// The contact numbers of the person.
    /// </summary>
    public HashSet<string> ContactNumbers { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    public Person()
    {
        Id = Guid.NewGuid();
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        ContactNumbers = [];
    }
}

