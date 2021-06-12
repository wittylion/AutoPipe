using System.Linq;
using FluentAssertions;
using Moq;
using Pipelines.Implementations.Pipelines;
using Pipelines.Implementations.Processors;
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

            await pipelineExecutor.Execute(string.Empty).ConfigureAwait(false);

            pipelineRunner.Verify(
                runner => runner.RunPipeline(
                    It.IsAny<IPipeline>(), It.IsAny<object>()
                ), Times.AtLeastOnce);
        }

        [Fact]
        public async void PipelineExecutor_Passes_Exact_Parameters_When_Executed()
        {
            var property = nameof(PipelineExecutor_Passes_Exact_Parameters_When_Executed);
            var processor = ActionProcessor.FromAction<Bag>(ctx => ctx.SetOrAddProperty(property, true));
            var pipeline = PredefinedPipeline.FromProcessors(processor);
            var pipelineExecutor = new PipelineExecutor(pipeline);

            var args = new Bag();
            await pipelineExecutor.Execute(args).ConfigureAwait(false);

            args.GetPropertyValueOrDefault(property, false)
                .Should()
                .BeTrue("context should have set up a property in passed context");
        }

        [Fact]
        public async void PipelineExecutor_Returns_Exact_Value_When_Query_Is_Executed()
        {
            var propertyValue = nameof(PipelineExecutor_Passes_Exact_Parameters_When_Executed);
            var processor = ActionProcessor.FromAction<Backpack<string>>(ctx =>
                ctx.SetResultWithInformation(propertyValue, "Result is set."));
            var pipeline = PredefinedPipeline.FromProcessors(processor);
            var pipelineExecutor = new PipelineExecutor(pipeline);
            var args = new Backpack<string>();

            (await pipelineExecutor.Execute(args))
                .Should()
                .Be(propertyValue);
        }
    }
}