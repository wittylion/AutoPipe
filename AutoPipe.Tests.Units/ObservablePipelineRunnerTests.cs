using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class ObservablePipelineRunnerTests
    {
        [Fact]
        public async void RunPipeline_Executes_Actions_On_Running_Is_Completed()
        {
            bool started = false;
            
            // The runner to be tested.
            Runner runner = new Runner(onPipelineStart: (e) => started = true);

            await runner.Run(Pipeline.Empty, new Bag()).ConfigureAwait(false);

            started.Should().BeTrue("because method must trigger the flag");
        }

        [Fact]
        public async void RunProcessor_Executes_Actions_On_Running_Is_Completed()
        {
            bool started = false;

            // The runner to be tested.
            Runner runner = new Runner(onProcessorStart: (e) => started = true);

            await runner.Run(Processor.From(() => { }), new Bag()).ConfigureAwait(false);

            started.Should().BeTrue("because method must trigger the flag");
        }
    }
}