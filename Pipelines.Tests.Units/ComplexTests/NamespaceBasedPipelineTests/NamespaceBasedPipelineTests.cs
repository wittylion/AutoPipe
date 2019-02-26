﻿using System.Linq;
using FluentAssertions;
using Pipelines.Implementations.Reflection;
using Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest;
using Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest;
using Xunit;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests
{
    public class NamespaceBasedPipelineTests
    {
        protected internal static readonly string InitialTestFolder = "Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest";
        protected internal static readonly string OrderTestFolder = "Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest";

        [Fact]
        public void NamespaceBasedPipeline_Should_Have_Processors_When_Creating_From_Related_Test_Folder()
        {
            new NamespaceBasedPipeline(InitialTestFolder)
                .GetProcessors()
                .Should()
                .NotBeEmpty("because there are processors in folder");
        }

        [Fact]
        public void NamespaceBasedPipeline_Should_Have_Processor1_When_Creating_From_Related_Test_Folder()
        {
            new NamespaceBasedPipeline(InitialTestFolder)
                .GetProcessors()
                .Should()
                .Contain(x => x is TestProcessor1, "the test processor of type TestProcessor1 is defined in the folder");
        }
        
        [Fact]
        public void NamespaceBasedPipeline_Should_Create_Processors_Using_Order_Attribute_When_Creating_From_Related_Test_Folder()
        {
            var processors = new NamespaceBasedPipeline(InitialTestFolder)
                .GetProcessors();

            processors.ElementAt(0)
                .Should()
                .BeOfType<TestZeroProcessor>("it has an order attribute equal to 0 which is the smallest value");
        }
        
        [Fact]
        public void NamespaceBasedPipeline_Should_Create_Processors_Of_Same_Order_Attribute_Sorted_By_Name_When_Creating_From_Related_Test_Folder()
        {
            var processors = new NamespaceBasedPipeline(OrderTestFolder)
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
            new NamespaceBasedPipeline("Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.NamespaceDoesNotExist")
                .GetProcessors()
                .Should().BeEmpty("the namespace does not exist");
        }
    }
}