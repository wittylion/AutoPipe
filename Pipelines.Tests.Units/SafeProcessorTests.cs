using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class SafeProcessorTests
    {
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Pipeline_Context_Has_Aborted_Parameter_Set_To_True()
        {
            var reachedExecution = false;
            var args = new PipelineContext()
            {
                IsAborted = true
            };
            var processor = new TestProcessor(() => reachedExecution = true);
            await processor.Execute(args);
            reachedExecution.Should().BeFalse("pipeline was aborted");
        }

        [Fact]
        public async Task Safe_Execution_Is_Reached_When_Pipeline_Context_Has_Aborted_Parameter_Set_To_False()
        {
            var reachedExecution = false;
            var args = new PipelineContext()
            {
                IsAborted = false
            };
            var processor = new TestProcessor(() => reachedExecution = true);
            await processor.Execute(args);
            reachedExecution.Should().BeTrue("pipeline was not aborted");
        }
    }
}