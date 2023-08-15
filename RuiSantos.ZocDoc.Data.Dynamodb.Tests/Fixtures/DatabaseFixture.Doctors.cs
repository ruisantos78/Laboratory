using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

partial class DatabaseFixture {
    
    private IEnumerable<Task<Doctor>> CreeateDoctorsAsync() => new[]
    {
        new DoctorBuilder(Client)
            .WithDoctor("ABC001", "Orli", "Waller", "iam.dictum@protonmail.edu")
            .WithSpecialty("Cardiology")
            .WithContactNumber("1-516-872-1630")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC002", "Xyla", "Miranda", "etiam.ligula@hotmail.com")
            .WithSpecialty("Cardiology", "Dermatology")
            .WithContactNumber("1-356-676-7417")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC003", "John", "Doe", "john.doe@example.com")
            .WithSpecialty("Pediatrics")
            .WithContactNumber("1-123-456-7890")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC004", "Jane", "Smith", "jane.smith@example.com")
            .WithSpecialty("Gynecology", "Obstetrics")
            .WithContactNumber("1-987-654-3210")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC005", "David", "Johnson", "david.johnson@example.com")
            .WithSpecialty("Orthopedics")
            .WithContactNumber("1-888-555-1234")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC006", "Sarah", "Brown", "sarah.brown@example.com")
            .WithSpecialty("Neurology")
            .WithContactNumber("1-777-888-9999")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC007", "Michael", "Anderson", "michael.anderson@example.com")
            .WithSpecialty("Ophthalmology")
            .WithContactNumber("1-555-444-3333")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC008", "Emily", "Wilson", "emily.wilson@example.com")
            .WithSpecialty("Psychiatry")
            .WithContactNumber("1-222-333-4444")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC009", "Daniel", "Thompson", "daniel.thompson@example.com")
            .WithSpecialty("Urology")
            .WithContactNumber("1-666-777-8888")
            .BuildAsync(),

            new DoctorBuilder(Client)
            .WithDoctor("ABC010", "Olivia", "Miller", "olivia.miller@example.com")
            .WithSpecialty("Dentistry")
            .WithContactNumber("1-999-888-7777")
            .BuildAsync()
    };   
}