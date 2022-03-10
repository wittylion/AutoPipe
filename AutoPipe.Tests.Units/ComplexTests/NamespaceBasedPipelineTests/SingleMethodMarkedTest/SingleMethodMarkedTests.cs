using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SingleMethodMarkedTest
{
    public class SingleMethodMarkedTests
    {
        [Fact]
        public void The_Method_Marked_With_Run_Attribute_In_A_Non_Marked_Class_Should_Return_The_Result()
        {
            var bag = new NamespacePipeline().RunSync();
            bag.StringResult().Should().NotBeEmpty().And.BeEquivalentTo(CustomProcessor.guid.ToString());
        }
    }
}
