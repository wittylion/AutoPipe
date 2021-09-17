using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using AutoPipe.Modifications;

namespace AutoPipe.Tests.Units
{
    public class PipelineExtensionsTests
    {
        [Fact]
        public void CacheInMemoryForHours_When_Lazy_Loading_Used_Should_Not_Return_Processors_Twice_Within_Test_Running()
        {
            var pipeline = new Mock<IPipeline>();
            var cached = pipeline.Object.CacheInMemoryForHours(1);

            cached.GetProcessors(); // Caching call
            cached.GetProcessors(); // Extra call to check cache

            pipeline.Verify(p => p.GetProcessors(), Times.Once,
                "The call to cached pipeline, should be only executed once, because the processors are expected to be cached for hour.");
        }

        [Fact]
        public async void CacheInMemoryForPeriod_When_Lazy_Loading_Used_Should_Update_Processors_Twice_Within_Test_Running()
        {
            var waitPeriod = TimeSpan.FromMilliseconds(100);
            var pipeline = new Mock<IPipeline>();
            var cached = pipeline.Object.CacheInMemoryForPeriod(waitPeriod);

            cached.GetProcessors(); // Caching call
            await Task.Delay(waitPeriod.Add(TimeSpan.FromMilliseconds(100))).ConfigureAwait(false);
            cached.GetProcessors(); // Extra call to check cache

            pipeline.Verify(p => p.GetProcessors(), Times.Exactly(2),
                "The call to cached pipeline, should be executed twice, because the processors are expected to be expired in 100 milliseconds.");
        }

        [Fact]
        public void CacheInMemory_When_Lazy_Loading_Used_Should_Not_Update_Processors_After_First_Run()
        {
            List<IProcessor> processors = new List<IProcessor>
            {
                new TestProcessor(() => { })
            };

            var pipeline = processors.ToPipeline().CacheInMemory();
            pipeline.GetProcessors();

            processors.Add(new TestProcessor(() => { }));

            pipeline.GetProcessors().Should().HaveCount(1, "updated list should not affect cached pipeline");
        }

        [Fact]
        public void CacheInMemory_When_Lazy_Loading_Not_Used_Should_Not_Update_Processors_On_First_Run()
        {
            List<IProcessor> processors = new List<IProcessor>
            {
                new TestProcessor(() => { })
            };

            var pipeline = processors.ToPipeline().CacheInMemory(false);
            
            processors.Add(new TestProcessor(() => { }));

            pipeline.GetProcessors().Should().HaveCount(1, "updated list should not affect cached pipeline");
        }

        [Fact]
        public void Modify_When_Configuration_Is_Passed_Should_Return_New_Processors_Instead_Of_Old()
        {
            IProcessor processor1 = new Mock<IProcessor>().Object;
            IProcessor processor2 = new Mock<IProcessor>().Object;
            IProcessor processor3 = new Mock<IProcessor>().Object;
            IProcessor processor4 = new Mock<IProcessor>().Object;
            IModificationConfiguration configuration =
                new ModificationConfigurationFacade(new[] {
                new SubstituteProcessorModification(processor1.GetMatcher(), processor3.ThenProcessor(processor3)),
                new SubstituteProcessorModification(processor2.GetMatcher(), processor4.ThenProcessor(processor3)),
                });

            processor1.ThenProcessor(processor2).ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor3, processor3, processor4, processor3);
        }
    }
}