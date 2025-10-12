using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace RuiSantos.Labs.Infrastructure.Tests.Extensions.FluentAssertions;

public class JTokenAssertions(JToken? subject)
    : ReferenceTypeAssertions<JToken?, JTokenAssertions>(subject, AssertionChain.GetOrCreate())
{
    protected override string Identifier => nameof(JToken);

    public AndConstraint<JTokenAssertions> Be<TValue>(TValue expected)
    {
        using var _ = new AssertionScope();
        
        Subject.Should().NotBeNull("Expected JSON token to have value {0}, but the element was <null>", expected);
        
        var subject = Subject!;
        subject.Value<TValue>().Should().Be(expected, "Expected JSON property {0} to have value {1}, but found {2}", subject.Path, expected, subject.Value<string>());

        return new AndConstraint<JTokenAssertions>(this);
    }

    public AndConstraint<JTokenAssertions> BeEquivalentTo<TValue>(IEnumerable<TValue> expected)
    {
        using var _ = new AssertionScope();
        
        Subject.Should().NotBeNull("Expected JSON token to have value {0}, but the element was <null>", expected);
        
        var subject = Subject!;
        subject.Values<TValue>().Should().BeEquivalentTo(expected, "Expected JSON property {0} to have value {1}, but found {2}", subject.Path, expected, subject.Values<string>());

        return new AndConstraint<JTokenAssertions>(this);
    }

    public JToken HaveChild(string element)
    {
        using var _ = new AssertionScope();
        
        Subject.Should().NotBeNull("Expected JSON token was <null>");
        
        var subject = Subject!;
        subject[element].Should().NotBeNull("Expected JSON token child '{0}' was <null>", element);

        return subject[element]!;
    }
}

