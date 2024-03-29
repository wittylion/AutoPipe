﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class PipelineRunnerTests
    {
        [Fact]
        public async void PipelineRunner_Uses_Declared_Processors_In_Pipeline()
        {
            var pipelineRunner = new Runner();
            var pipeline = new Mock<IPipeline>();
            
            pipeline.Setup(x => x.GetProcessors()).Returns(Enumerable.Empty<IProcessor>());

            await pipeline.Object.Run(runner: pipelineRunner).ConfigureAwait(false);

            pipeline.Verify(x => x.GetProcessors(), Times.AtLeastOnce);
        }

        [Fact]
        public async void PipelineRunner_Does_Not_Execute_When_Processors_Collection_Is_Null()
        {
            var pipelineRunner = new Runner();
            var pipeline = new Mock<IPipeline>();

            pipeline.Setup(x => x.GetProcessors()).Returns((IEnumerable<IProcessor>) null);

            await pipeline.Object.Run(runner: pipelineRunner).ConfigureAwait(false);
        }

        [Fact]
        public async void PipelineRunner_Executes_Processors_In_The_Collection_In_Exact_Order()
        {
            var pipelineRunner = new Runner();
            var executionSequence = new MockSequence();
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var a = mockRepository.Create<IProcessor>();
            var b = mockRepository.Create<IProcessor>();
            var c = mockRepository.Create<IProcessor>();

            a.InSequence(executionSequence).Setup(x => x.Run(It.IsAny<Bag>())).Returns(PipelineTask.CompletedTask);
            b.InSequence(executionSequence).Setup(x => x.Run(It.IsAny<Bag>())).Returns(PipelineTask.CompletedTask);
            c.InSequence(executionSequence).Setup(x => x.Run(It.IsAny<Bag>())).Returns(PipelineTask.CompletedTask);

            var pipeline = new Mock<IPipeline>();
            pipeline.Setup(x => x.GetProcessors()).Returns(new [] { a.Object, b.Object, c.Object });

            await pipeline.Object.Run(runner: pipelineRunner).ConfigureAwait(false);
        }
    }
}
