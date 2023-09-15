namespace RuiSantos.Labs.Client.Services;

[RegisterService(typeof(ProfilesOperations), InstanceType = InstanceType.Singleton)]
public interface IProfilesOperations
{
    IReadOnlyList<string> GetAllProfiles();
    IReadOnlyList<Operation> GetOperations(string profile);
    string GetRoute(Guid id);
}

public readonly record struct Operation(Guid Id, string DisplayName, string Route);

public class ProfilesOperations : IProfilesOperations
{
    private static readonly IReadOnlyList<ProfilesOperations> Items = new List<ProfilesOperations> {
        new() {
            Profile = "Administrator",
            Operations = new Operation[] {
                new(Guid.NewGuid(), "Medical specialties management", "/admin/specialties"),
            }
        },
        new() {
            Profile = "Doctor",
            Operations = new Operation[] {
                new(Guid.NewGuid(), "Doctor Operation 1", "doctor-operation-1"),
                new(Guid.NewGuid(), "Doctor Operation 2", "doctor-operation-2")
            }
        },
        new() {
            Profile = "Patient",
            Operations = new Operation[] {
                new(Guid.NewGuid(), "Patient Operation 1", "patient-operation-1"),
                new(Guid.NewGuid(), "Patient Operation 2", "patient-operation-2")
            }
        }
    };

    protected string Profile { get; init; } = string.Empty;

    protected IReadOnlyList<Operation> Operations { get; init; } = Array.Empty<Operation>();

    public IReadOnlyList<string> GetAllProfiles() =>
        Items.Select(x => x.Profile).ToList();

    public IReadOnlyList<Operation> GetOperations(string profile) =>
        Items.FirstOrDefault(x => x.Profile == profile)?.Operations ?? Array.Empty<Operation>();

    public string GetRoute(Guid id) =>
        Items.SelectMany(x => x.Operations).FirstOrDefault(x => x.Id == id).Route;
}