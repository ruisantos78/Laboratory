using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace RuiSantos.Labs.API.Tests;

internal static class HttpExtensions
{
	public static async Task<TContract> As<TContract>(this HttpContent content, ITestOutputHelper? output = null)
	{
		var stringContent = await content.ReadAsStringAsync();
		output?.WriteLine(stringContent);

		return JsonConvert.DeserializeObject<TContract>(stringContent)!;
	}

    public static async Task<TSchema> GetSchema<TSchema>(this HttpContent content, string root = "data", ITestOutputHelper? output = null)
        where TSchema: class
    {
        var stringContent = await content.ReadAsStringAsync();
        output?.WriteLine(stringContent);

        var token = JToken.Parse(stringContent);
        return token[root]?.ToObject<TSchema>() ?? Activator.CreateInstance<TSchema>();
    }

    public static async Task<JToken> GetTokenAsync(this HttpContent content, ITestOutputHelper? output = null)
    {
        var stringContent = await content.ReadAsStringAsync();
        output?.WriteLine(stringContent);

        return JToken.Parse(stringContent);
    }

    public static async Task<HttpResponseMessage> PostAsync(this HttpClient client,
		string url, object request, ITestOutputHelper? output = null)
	{
		var content = JsonConvert.SerializeObject(request);
        output?.WriteLine(content);

        var stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

        return await client.PostAsync(url, stringContent);
	}

    public static async Task<HttpResponseMessage> PutAsync(this HttpClient client,
    string url, object request, ITestOutputHelper? output = null)
    {
        var content = JsonConvert.SerializeObject(request);
        output?.WriteLine(content);

        var stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

        return await client.PutAsync(url, stringContent);
    }
}