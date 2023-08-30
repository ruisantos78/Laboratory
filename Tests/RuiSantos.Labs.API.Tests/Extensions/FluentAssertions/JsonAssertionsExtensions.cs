using Newtonsoft.Json.Linq;

namespace FluentAssertions;

public static class JsonAssertionsExtensions
{
    public static JTokenAssertions Should(this JToken? token)
    {
        return new JTokenAssertions(token);
    }
}

