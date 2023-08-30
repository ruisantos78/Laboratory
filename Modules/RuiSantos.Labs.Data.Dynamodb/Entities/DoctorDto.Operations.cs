using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Models;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;
using static RuiSantos.Labs.Data.Dynamodb.Entities.Data.DynamoOperations<RuiSantos.Labs.Core.Models.Doctor>;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class DoctorDto
{
    public static async Task<Doctor?> GetDoctorByLicenseAsync(IDynamoDBContext context, string license)
    {
        var result = await FindAllAsync<DoctorDto>(context, DoctorLicenseIndexName, license);
        return result.FirstOrDefault();
    }

    public static async Task<Doctor?> GetDoctorByAppointmentAsync(IDynamoDBContext context, Appointment appointment)
    {
        var appointmentEntity = await context.LoadAsync<AppointmentsDto>(appointment.Id);
        return await FindAsync<DoctorDto>(context, appointmentEntity.DoctorId);
    }

    public static async Task<List<Doctor>> GetDoctorsBySpecialtyAsync(IDynamoDBContext context, string specialty)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(specialty, new DynamoDBOperationConfig
        {
            IndexName = DoctorSpecialtyIndexName
        })
        .GetRemainingAsync();

        var doctorIds = specialties.Select(x => x.DoctorId as object).ToList();
        return await FindListAsync<DoctorDto>(context, doctorIds);
    }

    public static async Task SetDoctorAsync(IDynamoDBContext context, Doctor doctor)
    {     
        var specialtiesWriter = await GetDoctorSpecialtiesWriterAsync(context, doctor);
        await StoreAsync<DoctorDto>(context, doctor, specialtiesWriter);        
    }

    private static async Task<BatchWrite> GetDoctorSpecialtiesWriterAsync(IDynamoDBContext context, Doctor doctor)
    {
        var currentSpecialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id)
            .GetRemainingAsync();

        var excludeSpecialties = currentSpecialties.Where(x => !doctor.Specialties.Contains(x.Specialty));
        var includeSpecialties = doctor.Specialties.Except(currentSpecialties.Select(x => x.Specialty))
            .Select(specialty => new DoctorSpecialtyDto
            {
                DoctorId = doctor.Id,
                Specialty = specialty
            });

        var writer = context.CreateBatchWrite<DoctorSpecialtyDto>();
        writer.AddDeleteItems(excludeSpecialties);
        writer.AddPutItems(includeSpecialties);
        return writer;
    }
}
