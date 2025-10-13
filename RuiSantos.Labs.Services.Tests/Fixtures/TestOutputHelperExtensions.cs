using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Fixtures;

internal static class TestOutputHelperExtensions
{
    public static async Task HttpResponseAsync(this ITestOutputHelper outputHelper, HttpResponseMessage response, bool showSuccessfulResponses = false)
    {
        if (response.IsSuccessStatusCode && !showSuccessfulResponses)
            return;

        var output = await response.Content.ReadAsStringAsync();
        outputHelper.WriteLine(output);
    }
}