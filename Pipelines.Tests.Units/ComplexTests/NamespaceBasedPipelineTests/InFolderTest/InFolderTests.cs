using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InFolderTest
{
    public class InFolderTests
    {
        [Fact]
        public void NamespacePipeline_Should_Obtain_The_Calling_Assembly_Namespace_And_Get_Processors()
        {
            new NamespacePipeline()
                .GetProcessors()
                .Should()
                .NotBeEmpty("because there are processors in folder")
                .And
                .HaveCount(2)
                .And
                .ContainItemsAssignableTo<IProcessor>();
        }
    }

    class TestProcessor1 : IProcessor
    {
        public Task Run(object arguments)
        {
            return Task.CompletedTask;
        }
    }

    class TestProcessor2 : IProcessor
    {
        public Task Run(object arguments)
        {
            return Task.CompletedTask;
        }
    }
}
