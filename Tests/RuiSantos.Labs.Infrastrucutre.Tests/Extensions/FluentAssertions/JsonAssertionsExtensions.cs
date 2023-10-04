using Newtonsoft.Json.Linq;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Extensions.FluentAssertions;

public static class JsonAssertionsExtensions
{
    public static JTokenAssertions Should(this JToken? token)
    {
        return new JTokenAssertions(token);
    }
}

