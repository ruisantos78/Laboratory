using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Domain.Repositories;

public interface IDoctorRepository
{
    /// <summary>
    /// Finds doctor by license. This is the asynchronous version of the method. It returns a task that completes when the doctor is found or fails when it is not found.
    /// </summary>
    /// <param name="id">Id of the doctor.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that completes when the doctor is found or fails when it is not found. The task completes when the doctor is</returns>
    Task<Doctor?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Finds all doctors. Asynchronously invokes the search. Returns a task that represents the asynchronous operation.
    /// </summary>
    /// <param name="take">Limit the number of results returned</param>
    /// <param name="paginationToken">Pagination token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Pagination<Doctor>> FindAllAsync(int take, string? paginationToken, CancellationToken cancellationToken);

    /// <summary>
    /// Finds doctor by speciality. Asynchronously invokes the search. Returns a task that represents the asynchronous operation.
    /// </summary>
    /// <param name="specialty">The name of the speciality. E.g.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An object that represents the asynchronous operation. The task result contains a instance of the class containing the dao rows</returns>
    Task<IEnumerable<Doctor>> FindBySpecialityAsync(string specialty, CancellationToken cancellationToken);

    /// <summary>
    /// Finds doctor by appointments asynchronous. Returns collection of doctor. To get call GetDoctorByAppointmentsAsync ()
    /// </summary>
    /// <param name="appointments">Enumeration of appointments to find doctor by.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of doctor corresponding to specified appointments or empty list if not found ( null is returned</returns>
    Task<IEnumerable<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken);

    /// <summary>
    /// Finds doctor schedules by specialty and availability. This is the same as FindBySpecialty but returns schedules that are available for the period specified
    /// </summary>
    /// <param name="specialty">The specialty to search for</param>
    /// <param name="date">Specifies whether to search for schedules on or after the specified date.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The collection of s that match the criteria ; or an empty collection if no schedule is found with the specified criteria</returns>
    IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date, CancellationToken cancellationToken);

    Task<Doctor?> FindByLicenseAsync(string license, CancellationToken cancellationToken);
    
    /// <summary>
    /// Stores the doctor in the database. This is a blocking call. If the store fails to complete an exception will be thrown.
    /// </summary>
    /// <param name="doctor">The doctor to store. This should be a non - null reference</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A representing the asynchronous operation</returns>
    Task StoreAsync(Doctor doctor, CancellationToken cancellationToken);
}
