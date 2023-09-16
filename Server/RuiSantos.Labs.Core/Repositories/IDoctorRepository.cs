using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

public interface IDoctorRepository
{
    IAsyncEnumerable<Doctor> FindAllAsync(int take, string? from = null);

    /// <summary>
    /// Finds doctor by license. This is the asynchronous version of the method. It returns a task that completes when the doctor is found or fails when it is not found.
    /// </summary>
    /// <param name="license">Licensed version of the doctor.</param>
    /// <returns>A task that completes when the doctor is found or fails when it is not found. The task completes when the doctor is</returns>
    Task<Doctor?> FindAsync(string license);

    /// <summary>
    /// Finds doctor by speciality. Asynchronously invokes the search. Returns a task that represents the asynchronous operation.
    /// </summary>
    /// <param name="specialty">The name of the speciality. E. g.</param>
    /// <returns>A object that represents the asynchronous operation. The task result contains a instance of the class containing the dao rows</returns>
    Task<List<Doctor>> FindBySpecialityAsync(string specialty);
    
    /// <summary>
    /// Finds doctor by appointments asynchronous. Returns collection of doctor. To get call GetDoctorByAppointmentsAsync ()
    /// </summary>
    /// <param name="appointments">Enumeration of appointments to find doctor by.</param>
    /// <returns>List of doctor corresponding to specified appointments or empty list if not found ( null is returned</returns>
    Task<List<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments);

    /// <summary>
    /// Finds doctor schedules by specialty and availability. This is the same as FindBySpecialty but returns schedules that are available for the period specified
    /// </summary>
    /// <param name="specialty">The specialty to search for</param>
    /// <param name="date">Specifies whether to search for schedules on or after the specified date.</param>
    /// <returns>The collection of s that match the criteria ; or an empty collection if no schedule is found with the specified criteria</returns>
    IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date);

    /// <summary>
    /// Stores the doctor in the database. This is a blocking call. If the store fails to complete an exception will be thrown.
    /// </summary>
    /// <param name="doctor">The doctor to store. This should be a non - null reference</param>
    /// <returns>A representing the asynchronous operation</returns>
    Task StoreAsync(Doctor doctor);
    Task<long> CountAsync();
}
