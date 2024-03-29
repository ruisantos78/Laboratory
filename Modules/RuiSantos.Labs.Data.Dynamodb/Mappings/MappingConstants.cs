﻿namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

internal static class MappingConstants
{
    #region Table Names

    public const string AppointmentsTableName = "Appointments";
    public const string DoctorsTableName = "Doctors";
    public const string DoctorSpecialtiesTableName = "DoctorSpecialties";
    public const string DictionaryTableName = "Dictionary";
    public const string PatientsTableName = "Patients";

    #endregion

    #region Indexes Names

    public const string DoctorLicenseIndexName = "DoctorLicenseIndex";
    public const string DoctorAppointmentsIndexName = "DoctorAppointmentsIndex";
    public const string PatientAppointmentsIndexName = "PatientAppointmentsIndex";
    public const string PatientSocialSecurityNumberIndexName = "PatientSocialSecurityNumberIndex";
    public const string DoctorSpecialtyIndexName = "DoctorSpecialtyIndex";

    #endregion

    #region Attributes Names

    public const string AppointmentIdAttributeName = "AppointmentId";
    public const string DoctorIdAttributeName = "DoctorId";
    public const string PatientIdAttributeName = "PatientId";
    public const string LicenseAttributeName = "License";
    public const string SpecialtyAttributeName = "Specialty";
    public const string SourceAttributeName = "Source";
    public const string ValueAttributeName = "Value";
    public const string SocialSecurityNumberAttributeName = "SocialSecurityNumber";
    public const string AppointmentDateTimeAttributeName = "AppointmentDateTime";

    #endregion
}