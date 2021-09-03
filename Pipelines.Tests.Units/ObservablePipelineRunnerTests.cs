using System;
using FluentAssertions;
using Moq;
using Xunit;
using Pipelines.Observable;

namespace Pipelines.Tests.Units
{
    public class ObservablePipelineRunnerTests
    {
        [Fact]
        public async void RunPipeline_Executes_Actions_On_Running_Is_Completed()
        {
            bool completed = false;
            
            // The runner to be tested.
            ObservablePipelineRunner runner = new ObservablePipelineRunner();

            // Creating an implementation of observer.
            var mockObserver =
                new Mock<IRunnerObserver<PipelineInfo>>();
            mockObserver
                .Setup(observer => observer.OnCompleted(It.IsAny<PipelineInfo>()))
                .Callback(() => completed = true);

            // Creating an implementation of pipeline.
            Mock<IPipeline> mockPipeline = new Mock<IPipeline>();


            using (runner.Subscribe(mockObserver.Object))
            {
                await runner.Run(mockPipeline.Object, null).ConfigureAwait(false);
            }


            completed.Should().BeTrue("because method on complete must trigger the flag");
        }

        [Fact]
        public async void Subscribe_Returns_A_Disposable_Object_Which_Removes_The_Subsriber_From_The_Collection()
        {
            bool completed = false;

            // The runner to be tested.
            ObservablePipelineRunner runner = new ObservablePipelineRunner();

            // Creating an implementation of observer.
            var mockObserver =
                new Mock<IRunnerObserver<PipelineInfo>>();
            mockObserver
                .Setup(observer => observer.OnCompleted(It.IsAny<PipelineInfo>()))
                .Callback(() => completed = true);

            // Creating an implementation of pipeline.
            Mock<IPipeline> mockPipeline = new Mock<IPipeline>();
            

            runner.Subscribe(mockObserver.Object).Dispose();
            await runner.Run(mockPipeline.Object, null).ConfigureAwait(false);


            completed.Should().BeFalse("because the subscriber was disposed before the RunPipeline was called");
        }

        [Fact]
        public async void OnError_Should_Be_Called_When_Error_Is_Not_Handled_In_Pipeline()
        {
            Exception exception = new NotImplementedException("Test exception");

            // The runner to be tested.
            ObservablePipelineRunner runner = new ObservablePipelineRunner();

            // Creating an implementation of observer.
            var mockObserver =
                new Mock<IRunnerObserver<PipelineInfo>>();
            mockObserver
                .Setup(observer => observer.OnError(It.Is<Exception>(er => er == exception), It.IsAny<PipelineInfo>()))
                .Verifiable("The exception thrown must be the same as specified in test method");

            // Creating a processor that throws an exception.
            Mock<IProcessor> mockProcessor = new Mock<IProcessor>();
            mockProcessor.Setup(x => x.Run(It.IsAny<object>())).Callback(() => throw exception);

            // Creating an implementation of pipeline.
            IPipeline mockPipeline = Pipeline.From(mockProcessor.Object);


            using (runner.Subscribe(mockObserver.Object))
            {
                await runner.Run(mockPipeline, null).ConfigureAwait(false);
            }


            mockObserver.Verify();
        }
    }
}