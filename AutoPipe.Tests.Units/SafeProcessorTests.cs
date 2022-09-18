using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class SafeProcessorTests
    {
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Pipeline_Context_Has_Ended_Parameter_Set_To_True()
        {
            var reachedExecution = false;
            var bag = new Bag(debug: true)
            {
                Ended = true
            };
            var processor = Processor.From(() => reachedExecution = true);
            await processor.Run(bag, Runner.Instance).ConfigureAwait(false);
            reachedExecution.Should().BeFalse(Settings.Explanation, "pipeline was ended", bag.Summary());
        }

        [Fact]
        public async Task Safe_Execution_Is_Reached_When_Pipeline_Context_Has_Ended_Parameter_Set_To_False()
        {
            var reachedExecution = false;
            var processor = Processor.From(() => reachedExecution = true);
            await Bag.Create().Run(processor).ConfigureAwait(false);
            reachedExecution.Should().BeTrue("pipeline was not ended");
        }
    }
}