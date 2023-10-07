using FluentAssertions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Tests.Asserts.Builders;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.Doctor;

public class GetAllDoctorsAsyncTests
{
    [Theory(DisplayName ="A list N of doctors should be returned with possibility to get the next N doctors ordered by license number")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetAllDoctorsAsync_FromRecord(int pageNumber)
    {
        // Arrange
        var asserts = new DoctorsServiceAsserts();

        var doctors = Enumerable.Range(1, 10).Select(i => new DoctorBuilder().With(license: $"AAA{i:000}").Build());
        var paginationToken = pageNumber > 0 ? Convert.ToBase64String(Guid.NewGuid().ToByteArray()) : default;

        var expected = new Pagination<Core.Models.Doctor>
        {
            Models = doctors.Skip(5 * pageNumber).Take(5).ToArray(),
            PaginationToken = pageNumber < 2 ? Convert.ToBase64String(Guid.NewGuid().ToByteArray()) : default
        };

        asserts.OnFindAllAsyncReturns(5, paginationToken, expected);

        // Act
        var service = asserts.GetService();
        var result = await service.GetAllDoctorsAsync(5, paginationToken);

        // Assert
        result.Should().Be(expected);
    }
}