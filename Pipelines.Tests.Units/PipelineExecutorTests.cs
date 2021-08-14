using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineExecutorTests
    {
        [Fact]
        public async void PipelineExecutor_Uses_Exact_PipelineRunner_When_Being_Executed()
        {
            var pipelineRunner = new Mock<Runner>();
            var pipeline = new Mock<IPipeline>();
            var pipelineExecutor = new PipelineExecutor(pipeline.Object, pipelineRunner.Object);

            pipeline.Setup(x => x.GetProcessors()).Returns(Enumerable.Empty<IProcessor>());

            await pipelineExecutor.Run(string.Empty).ConfigureAwait(false);

            pipelineRunner.Verify(
                runner => runner.Run(
                    It.IsAny<IPipeline>(), It.IsAny<object>()
                ), Times.AtLeastOnce);
        }

        [Fact]
        public async void PipelineExecutor_Passes_Exact_Parameters_When_Executed()
        {
            var property = nameof(PipelineExecutor_Passes_Exact_Parameters_When_Executed);
            var processor = Processor.From<Bag>(ctx => ctx.Set(property, true));
            var pipeline = Pipeline.From(processor);
            var pipelineExecutor = new PipelineExecutor(pipeline);

            var args = new Bag();
            await pipelineExecutor.Run(args).ConfigureAwait(false);

            args.Get(property, false)
                .Should()
                .BeTrue("context should have set up a property in passed context");
        }

        [Fact]
        public async void PipelineExecutor_Returns_Exact_Value_When_Query_Is_Executed()
        {
            var propertyValue = nameof(PipelineExecutor_Passes_Exact_Parameters_When_Executed);
            var processor = Processor.From<Bag>(ctx =>
                ctx.SetResultWithInformation(propertyValue, "Result is set."));
            var pipeline = Pipeline.From(processor);
            var pipelineExecutor = new PipelineExecutor(pipeline);
            var args = new Bag();

            (await pipelineExecutor.Make<string>(args))
                .Should()
                .Be(propertyValue);
        }
    }
}