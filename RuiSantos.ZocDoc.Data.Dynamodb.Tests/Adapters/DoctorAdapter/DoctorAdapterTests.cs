using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Adapters;

public partial class DoctorAdapterTests: IClassFixture<DatabaseFixture>
{
    private readonly DoctorAdapter doctorAdapter;
    private readonly List<Doctor> doctors;

    public DoctorAdapterTests(DatabaseFixture database)
    {
        this.doctorAdapter = new(database.Client);
        this.doctors = database.Doctors;
    }
}
