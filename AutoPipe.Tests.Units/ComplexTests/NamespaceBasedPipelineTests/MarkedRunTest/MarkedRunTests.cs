using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.MarkedRunTest
{
    public class MarkedRunTests
    {
        [Fact]
        public void Executing_This_Namespace_Should_Execute_A_Class_Marked_With_Run()
        {
            new NamespacePipeline().RunSync().GetResult<object>(fallbackValue: null).Should().NotBeNull();
        }
    }

    [Run]
    public class ThisOneIsMarkedRun
    {
        [Run]
        public object GetResult()
        {
            return new object();
        }
    }
}
