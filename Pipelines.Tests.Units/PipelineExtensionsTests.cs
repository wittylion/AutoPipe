using System;
using System.Threading.Tasks;
using Moq;
using Pipelines.ExtensionMethods;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineExtensionsTests
    {
        [Fact]
        public void CacheInMemoryForHours_Should_Not_Return_Processors_Twice_Within_Test_Running()
        {
            var pipeline = new Mock<IPipeline>();
            var cached = pipeline.Object.CacheInMemoryForHours(1);

            cached.GetProcessors(); // Caching call
            cached.GetProcessors(); // Extra call to check cache

            pipeline.Verify(p => p.GetProcessors(), Times.Once,
                "The call to cached pipeline, should be only executed once, because the processors are expected to be cached for hour.");
        }

        [Fact]
        public async void CacheInMemoryForPeriod_Should_Update_Processors_Twice_Within_Test_Running()
        {
            var waitPeriod = TimeSpan.FromMilliseconds(100);
            var pipeline = new Mock<IPipeline>();
            var cached = pipeline.Object.CacheInMemoryForPeriod(waitPeriod);

            cached.GetProcessors(); // Caching call
            await Task.Delay(waitPeriod.Add(TimeSpan.FromMilliseconds(100)));
            cached.GetProcessors(); // Extra call to check cache

            pipeline.Verify(p => p.GetProcessors(), Times.Exactly(2),
                "The call to cached pipeline, should be executed twice, because the processors are expected to be expired in 100 milliseconds.");
        }
    }
}