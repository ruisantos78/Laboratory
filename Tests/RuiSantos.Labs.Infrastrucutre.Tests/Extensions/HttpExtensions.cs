using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Extensions;

internal static class HttpExtensions
{
    public static async Task<TContract> GetContractAsync<TContract>(this HttpContent content,
        ITestOutputHelper? output = null)
    {
        var stringContent = await content.ReadAsStringAsync();
        output?.WriteLine(stringContent);

        return JsonConvert.DeserializeObject<TContract>(stringContent)!;
    }

    public static async Task<JToken> GetTokenAsync(this HttpContent content, ITestOutputHelper? output = null)
    {
        var stringContent = await content.ReadAsStringAsync();
        output?.WriteLine(stringContent);

        return JToken.Parse(stringContent);
    }

    public static Task<HttpResponseMessage> PostAsync(this HttpClient client,
        string url, object request, ITestOutputHelper? output = null)
    {
        var content = JsonConvert.SerializeObject(request);
        output?.WriteLine(content);

        var stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
        return client.PostAsync(url, stringContent);
    }
}