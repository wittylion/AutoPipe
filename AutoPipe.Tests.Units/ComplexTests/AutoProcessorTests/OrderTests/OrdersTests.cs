using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests
{
    public class OrdersTests
    {
        [Theory]
        [MemberData(nameof(OrderMethodsDataSets.GetProcessorsAndOrder), MemberType = typeof(OrderMethodsDataSets))]
        public void Check(AutoProcessor processor, IEnumerable<string> expected)
        {
            processor.GetMethodsToExecute().Select(x => x.Name).Should().Equal(expected);
        }
    }
}
