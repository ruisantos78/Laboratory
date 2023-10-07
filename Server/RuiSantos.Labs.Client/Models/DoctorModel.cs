namespace RuiSantos.Labs.Client.Models;

public class DoctorModel
{
    public string License { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public IReadOnlyList<string> Contacts { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Specialties { get; init; } = Array.Empty<string>();

    public string FullName => $"{LastName}, {FirstName}";
}
