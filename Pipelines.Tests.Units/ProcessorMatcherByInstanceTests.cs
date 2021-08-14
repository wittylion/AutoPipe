using FluentAssertions;
using Moq;
using Xunit;
using Pipelines.Modifications;

namespace Pipelines.Tests.Units
{
    public class ProcessorMatcherByInstanceTests
    {
        [Fact]
        public void Instance_ReturnsTrue_WhenTheSameInstanceIsPassed()
        {
            var processor = new Mock<IProcessor>().Object;
            new ProcessorMatcherByInstance(processor).Matches(processor).Should().BeTrue();
        }

        [Fact]
        public void Instance_ReturnsFalse_WhenOtherInstanceIsPassed()
        {
            var processor1 = new Mock<IProcessor>().Object;
            var processor2 = new Mock<IProcessor>().Object;
            new ProcessorMatcherByInstance(processor1).Matches(processor2).Should().BeFalse();
        }
    }
}
