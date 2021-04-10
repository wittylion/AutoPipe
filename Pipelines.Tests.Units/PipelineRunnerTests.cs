using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineRunnerTests
    {
        [Fact]
        public async void PipelineRunner_Uses_Declared_Processors_In_Pipeline()
        {
            var pipelineRunner = new PipelineRunner();
            var pipeline = new Mock<IPipeline>();
            
            pipeline.Setup(x => x.GetProcessors()).Returns(Enumerable.Empty<IProcessor>());

            await pipelineRunner.RunPipeline(pipeline.Object, string.Empty).ConfigureAwait(false);

            pipeline.Verify(x => x.GetProcessors(), Times.AtLeastOnce);
        }

        [Fact]
        public async void PipelineRunner_Does_Not_Execute_When_Processors_Collection_Is_Null()
        {
            var pipelineRunner = new PipelineRunner();
            var pipeline = new Mock<IPipeline>();

            pipeline.Setup(x => x.GetProcessors()).Returns((IEnumerable<IProcessor>) null);

            await pipelineRunner.RunPipeline(pipeline.Object, string.Empty).ConfigureAwait(false);
        }

        [Fact]
        public async void PipelineRunner_Executes_Processors_In_The_Collection_In_Exact_Order()
        {
            var pipelineRunner = new PipelineRunner();
            var executionSequence = new MockSequence();
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var a = mockRepository.Create<IProcessor>();
            var b = mockRepository.Create<IProcessor>();
            var c = mockRepository.Create<IProcessor>();

            a.InSequence(executionSequence).Setup(x => x.Execute(It.IsAny<object>())).Returns(PipelineTask.CompletedTask);
            b.InSequence(executionSequence).Setup(x => x.Execute(It.IsAny<object>())).Returns(PipelineTask.CompletedTask);
            c.InSequence(executionSequence).Setup(x => x.Execute(It.IsAny<object>())).Returns(PipelineTask.CompletedTask);

            var pipeline = new Mock<IPipeline>();
            pipeline.Setup(x => x.GetProcessors()).Returns(new [] { a.Object, b.Object, c.Object });

            await pipelineRunner.RunPipeline(pipeline.Object, string.Empty).ConfigureAwait(false);
        }
    }
}
