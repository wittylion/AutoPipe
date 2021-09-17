using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class SafeTypeProcessorTests
    {
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_There_Is_An_Incorrect_Type()
        {
            var processor = new Mock<SafeProcessor>();
            await processor.Object.Run(false).ConfigureAwait(false);
            processor.Verify(p => p.SafeRun(It.IsAny<Bag>()), Times.Never);
        }

        [Fact]
        public async Task Safe_Condition_Is_Reached_When_Type_Is_Correct()
        {
            var processor = new Mock<SafeProcessor>();
            await processor.Object.Run().ConfigureAwait(false);
            processor.Verify(p => p.SafeCondition(It.IsAny<Bag>()), Times.AtLeastOnce);
        }
        
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Safe_Condition_Returns_False()
        {
            var processor = new Mock<SafeProcessor>();
            processor.Setup(x => x.SafeCondition(It.IsAny<Bag>())).Returns(false);
            await processor.Object.Run().ConfigureAwait(false);
            processor.Verify(p => p.SafeRun(It.IsAny<Bag>()), Times.Never);
        }


        [Fact]
        public async Task Safe_Execution_Is_Reached_When_Safe_Condition_Returns_False()
        {
            var processor = new Mock<SafeProcessor>();
            processor.Setup(x => x.SafeCondition(It.IsAny<Bag>())).Returns(true);
            await processor.Object.Run().ConfigureAwait(false);
            processor.Verify(p => p.SafeRun(It.IsAny<Bag>()), Times.AtLeastOnce);
        }
        
        [Fact]
        public async Task Safe_Processor_Makes_A_Check_Of_Safe_Condition_Before_Doing_Safe_Execution()
        {
            var processor = new Mock<SafeProcessor>(MockBehavior.Strict);

            var executionSequence = new MockSequence();
            processor.InSequence(executionSequence).Setup(x => x.SafeCondition(It.IsAny<Bag>())).Returns(true);
            processor.InSequence(executionSequence).Setup(x => x.SafeRun(It.IsAny<Bag>())).Returns(PipelineTask.CompletedTask);

            await processor.Object.Run().ConfigureAwait(false);
        }
    }
}