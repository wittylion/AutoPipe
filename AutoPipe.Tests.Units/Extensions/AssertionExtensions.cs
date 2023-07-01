using FluentAssertions.Execution;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentAssertions.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoPipe.Tests.Units
{
    public static class AssertionExtensions
    {
        public static BagAssertions Should(this Bag bag)
        {
            return new BagAssertions(bag);
        }
    }

    public class BagAssertions : ReferenceTypeAssertions<Bag, BagAssertions>
    {
        public BagAssertions(Bag bag)
        {
            base.Subject = bag;
        }

        protected override string Identifier => "bag";

        public AndConstraint<BagAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            var baseProperties = new List<string>() { Bag.DebugProperty, Bag.ThrowOnMissingProperty, Bag.EndedProperty, Bag.ResultProperty, Bag.ServiceProviderProperty, };
            var message = "Expected {context:bag} to be empty{reason}, but was not.";
            var props = Subject.Keys.Except(baseProperties);

            Execute.Assertion.ForCondition(props.Count() == 0).BecauseOf(because, becauseArgs).FailWith(PrepareMessage(message), Subject.Summary(), string.Join(", ", Subject.Select(x => $"{x.Key}={x.Value}")));
            return new AndConstraint<BagAssertions>(this);
        }

        public string PrepareMessage(string baseMessage, bool includeSummary = true, bool includeProperties = true)
        {
            var message = new StringBuilder(baseMessage);

            if (includeSummary)
            {
                message.AppendLine().AppendLine()
                .Append("Review bag summary:")
                .AppendLine().AppendLine()
                .Append("{0}");
            }

            if (includeProperties)
            {
                message
                .AppendLine().AppendLine()
                .Append("Review properties:")
                .AppendLine().AppendLine()
                .Append("{1}");
            }

            return message.ToString();
        }
    }
}