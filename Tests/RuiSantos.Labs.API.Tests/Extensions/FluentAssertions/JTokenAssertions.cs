using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace FluentAssertions;

public class JTokenAssertions: ReferenceTypeAssertions<JToken?, JTokenAssertions>
{
    public JTokenAssertions(JToken? subject) : base(subject)
    {
    }

    protected override string Identifier => nameof(JToken);

    public AndConstraint<JTokenAssertions> Be<TValue>(TValue expected)
    {
        Execute.Assertion
                .ForCondition(Subject is not null)
                .FailWith("Expected JSON token to have value {0}, but the element was <null>.", expected);

        Execute.Assertion
            .ForCondition(Subject?.Value<TValue>()?.Equals(expected) is true)            
            .FailWith("Expected JSON property {0} to have value {1}, but found {2}.",
                Subject?.Path, expected, Subject?.Value<string>());

        return new AndConstraint<JTokenAssertions>(this);
    }

    public AndConstraint<JTokenAssertions> BeEquivalentTo<TValue>(IEnumerable<TValue> expected)
    {
        Execute.Assertion
                .ForCondition(Subject is not null)
                .FailWith("Expected JSON token to have value {0}, but the element was <null>.", expected);

        Execute.Assertion
            .ForCondition(this.Subject?.Values<TValue>()?.All(x => expected.Contains(x)) is true)            
            .FailWith("Expected JSON property {0} to have value {1}, but found {2}.",
                Subject?.Path, expected, Subject?.Values<string>());

        return new AndConstraint<JTokenAssertions>(this);
    }

    public JToken HaveChild(string element)
    {
        Execute.Assertion
            .ForCondition(Subject is not null)
            .FailWith("Expected JSON token was <null>.");

        Execute.Assertion
            .ForCondition(Subject?[element] is not null)
            .FailWith("Expected JSON token was <null>.");

        return Subject![element]!;
    }
}

