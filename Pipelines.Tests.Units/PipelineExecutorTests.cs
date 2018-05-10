using System.Linq;
using Moq;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineExecutorTests
    {
        [Fact]
        public async void PipelineExecutor_Uses_Exact_PipelineRunner_When_Being_Executed()
        {
            var pipelineRunner = new Mock<PipelineRunner>();
            var pipeline = new Mock<IPipeline>();
            var pipelineExecutor = new PipelineExecutor(pipeline.Object, pipelineRunner.Object);

            pipeline.Setup(x => x.GetProcessors()).Returns(Enumerable.Empty<IProcessor>());

            await pipelineExecutor.Execute(string.Empty);

            pipelineRunner.Verify(
                runner => runner.RunPipeline(
                    It.IsAny<IPipeline>(), It.IsAny<object>()
                ), Times.AtLeastOnce);
        }

    }
}