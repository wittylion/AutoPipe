using FluentAssertions;
using Pipelines.Implementations.Processors;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class ProcessorMatcherByTypeTests
    {
        [Fact]
        public void Instance_ReturnsTrue_WhenTheSameTypeIsPassed()
        {
            var processor = new TestProcessor(() => { });
            new ProcessorMatcherByType(typeof(TestProcessor)).Matches(processor).Should().BeTrue();
        }

        [Fact]
        public void Instance_ReturnsFalse_WhenOtherTypeIsPassed()
        {
            var processor = new Processor(o => PipelineTask.CompletedTask);
            new ProcessorMatcherByType(typeof(TestProcessor)).Matches(processor).Should().BeFalse();
        }
    }
}
