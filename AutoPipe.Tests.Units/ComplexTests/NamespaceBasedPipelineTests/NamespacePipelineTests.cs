using System.Linq;
using FluentAssertions;
using Moq;
using AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest;
using AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests
{
    public class NamespacePipelineTests
    {
        protected internal static readonly string InitialTestFolder = "AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest";
        protected internal static readonly string OrderTestFolder = "AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest";
        protected internal static readonly string SkipTestFolder = "AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SkipTest";

        [Fact]
        public void NamespaceBasedPipeline_Should_Have_Processors_When_Creating_From_Related_Test_Folder()
        {
            new NamespacePipeline(InitialTestFolder)
                .GetProcessors()
                .Should()
                .NotBeEmpty("because there are processors in folder");
        }

        [Fact]
        public void NamespaceBasedPipeline_Should_Have_Processor1_When_Creating_From_Related_Test_Folder()
        {
            new NamespacePipeline(InitialTestFolder)
                .GetProcessors()
                .Should()
                .Contain(x => x is TestProcessor1, "the test processor of type TestProcessor1 is defined in the folder");
        }
        
        [Fact]
        public void NamespaceBasedPipeline_Should_Create_Processors_Using_Order_Attribute_When_Creating_From_Related_Test_Folder()
        {
            var processors = new NamespacePipeline(InitialTestFolder)
                .GetProcessors();

            processors.ElementAt(0)
                .Should()
                .BeOfType<TestZeroProcessor>("it has an order attribute equal to 0 which is the smallest value");
        }
        
        [Fact]
        public void NamespaceBasedPipeline_Should_Create_Processors_Of_Same_Order_Attribute_Sorted_By_Name_When_Creating_From_Related_Test_Folder()
        {
            var processors = new NamespacePipeline(OrderTestFolder)
                .GetProcessors();

            processors.ElementAt(0)
                .Should()
                .BeOfType<TestProcessorOrder1>("comparing strings this should be the first one")
                .And
                .NotBeOfType<TestProcessorOrder1Additional>("comparing strings this processor has word additional");
        }

        [Fact]
        public void NamespaceBasedPipeline_Should_Return_Empty_Enumerable_When_Namespace_Does_Not_Exist()
        {
            new NamespacePipeline("AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.NamespaceDoesNotExist")
                .GetProcessors()
                .Should().BeEmpty("the namespace does not exist");
        }

        [Fact]
        public void NamespaceBasedPipeline_Should_Run_With_Extension_Methods_Without_Errors()
        {
            var mockRunner = new Mock<Runner> {CallBase = true};

            new NamespacePipeline(OrderTestFolder).Run(runner: mockRunner.Object);
            var expectedCount = new NamespacePipeline(OrderTestFolder).GetProcessors().Count();

            mockRunner.Verify(x => x.Run(It.IsAny<IProcessor>(), It.IsAny<Bag>()),
                Times.Exactly(expectedCount), "Method run processor should be executed, since pipeline has processors");
        }

        [Fact]
        public void NamespaceBasedPipeline_Should_Not_Contain_Processor_With_SkipProcessor_Attribute()
        {
            new NamespacePipeline(SkipTestFolder).GetProcessors()
                .Should().Contain(x => x.GetType() == typeof(SkipTest.IncludedProcessor))
                .And
                .NotContain(x => x.GetType() == typeof(SkipTest.SkippedProcessor));
        }
    }
}