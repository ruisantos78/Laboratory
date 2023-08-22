using Newtonsoft.Json;
using Xunit.Abstractions;

namespace RuiSantos.ZocDoc.API.Tests;

internal static class HttpExtensions
{
	public static async Task<TContract> As<TContract>(this HttpContent content, ITestOutputHelper? output = null)
	{
		var stringContent = await content.ReadAsStringAsync();
		output?.WriteLine(stringContent);

		return JsonConvert.DeserializeObject<TContract>(stringContent)!;
	}

	public static async Task<HttpResponseMessage> PostAsync(this HttpClient client,
		string url, object request, ITestOutputHelper? output = null)
	{
		var content = JsonConvert.SerializeObject(request);
        output?.WriteLine(content);

        var stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

        return await client.PostAsync(url, stringContent);
	}
}