namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal static class ClassMapConstants
{
    public const string AppointmentsTableName = "Appointments";
    public const string DoctorsTableName = "Doctors";
    public const string DoctorSpecialtiesTableName = "DoctorSpecialties";
    public const string DomainListsTableName = "DomainLists";
    public const string PatientsTableName = "Patients";

    public const string DoctorLicenseIndexName = "DoctorLicenseIndex";
    public const string DoctorSpecialtyIndexName = "DoctorSpecialtyIndex";
    public const string DoctorAppointmentIndexName = "DoctorAppointmentIndex";
    public const string PatientAppointmentIndexName = "PatientAppointmentIndex";
    public const string PatientSocialSecurityNumberIndexName = "PatientSocialSecurityNumberIndex";

    public const string IdAttributeName = "Id";
    public const string AppointmentIdAttributeName  = "AppointmentId";
    public const string DoctorIdAttributeName = "DoctorId";
    public const string PatientIdAttributeName = "PatientId";    
    public const string LicenseAttributeName = "License";
    public const string SpecialtyAttributeName = "Specialty";        
    public const string SourceAttributeName = "Source";        
    public const string SocialSecurityNumberAttributeName = "SocialSecurityNumber";    
}