namespace RuiSantos.Labs.Core.Repositories;

public interface IMedicalSpecialityRepository 
{
    /// <summary>
    /// Gets the list of strings. This will be empty if there are no strings in the list. The caller is responsible for ensuring that the list is not null.
    /// </summary>
    /// <returns>A that can be used to monitor the operation and obtain the list of strings or null if there are no strings</returns>
    Task<IEnumerable<string>> GetAsync();

    /// <summary>
    /// Removes speciality from the list of specialities. This is a no - op if the speciality is not in the list.
    /// </summary>
    /// <param name="speciality">The name of the speciality to remove.</param>
    /// <returns>A task representing the asynchronous operation. When the task completes the property will contain the speciality that was removed</returns>
    Task RemoveAsync(string speciality);

    /// <summary>
    /// Adds specialties to the list. This is done in a background thread to avoid blocking the UI thread
    /// </summary>
    /// <param name="specialties">The list of specialties to add</param>
    /// <returns>A task that completes when the list is added to the list and returns the result of the task ( s</returns>
    Task AddAsync(IEnumerable<string> specialties);
}