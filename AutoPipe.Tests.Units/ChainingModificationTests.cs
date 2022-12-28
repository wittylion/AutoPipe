﻿using FluentAssertions;
using System.Linq;
using Xunit;
using AutoPipe.Modifications;

namespace AutoPipe.Tests.Units
{
    public class ChainingModificationTests
    {
        [Fact]
        public void AddLast_ShouldWorkProperly_WithGenericType()
        {
            Pipeline.Empty.Modify(x => x.AddLast<ActionProcessor>())
                .GetProcessors().Should().AllBeOfType<ActionProcessor>();
        }

        [Fact]
        public void After_ShouldWorkProperly_WithGenericType()
        {
            var processor1 = Processor.Empty;

            var configuration = new ChainingModification()
                .After<ActionProcessor, ActionProcessor>()
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().HaveCount(2).And.Subject.First().Should().Be(processor1);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithGenericOriginalAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .After<TestProcessor>(processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithGenericOriginalAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = new ChainingModification()
                .After<TestProcessor>(processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithTypeOfOriginalAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .After(typeof(TestProcessor), processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithTypeOfOriginalAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = new ChainingModification()
                .After(typeof(TestProcessor), processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithOriginalInstanceAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .After(processor1, processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2);
        }

        [Fact]
        public void After_ShouldWorkProperly_WithOriginalInstanceAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = new ChainingModification()
                .After(processor1, processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithGenericType()
        {
            var processor1 = Processor.Empty;

            var configuration = new ChainingModification()
                .Before<ActionProcessor, TestProcessor>()
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().HaveCount(2).And.Subject.Last().Should().Be(processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithGenericOriginalAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .Before<TestProcessor>(processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithGenericOriginalAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = new ChainingModification()
                .Before<TestProcessor>(processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor3, processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithTypeOfOriginalAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .Before(typeof(TestProcessor), processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithTypeOfOriginalAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = new ChainingModification()
                .Before(typeof(TestProcessor), processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor3, processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithOriginalInstanceAndPassedProcessor()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();

            var configuration = new ChainingModification()
                .Before(processor1, processor2)
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor1);
        }

        [Fact]
        public void Before_ShouldWorkProperly_WithOriginalInstanceAndPassedProcessors()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure()
                .Before(processor1, processor2.ThenProcessor(processor3))
                .GetConfiguration();

            processor1.ToAnArray().ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor3, processor1);
        }

        [Fact]
        public void Remove_ShouldWorkProperly_WithOriginalInstance()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure()
                .Remove(processor1)
                .GetConfiguration();

            processor1.ThenProcessor(processor2).ThenProcessor(processor3).ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor2, processor3);
        }

        [Fact]
        public void Insert_ShouldWorkProperly_WhenThreeInstancesArePassedIntoEmptyPipeline()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure(
                    x => x.Insert(0, processor1, processor2, processor3)
                )
                .GetConfiguration();

            Pipeline.Empty.Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void Insert_ShouldWorkProperly_WhenTwoInstancesArePassedIntoOneProcessorPipeline()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure(
                    x => x.Insert(0, processor1, processor2)
                )
                .GetConfiguration();

            Pipeline.From(processor3).Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void Insert_ShouldWorkProperly_WhenTwoInstancesArePassedInDifferentPlaces()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure(
                    x => x.Insert(0, processor1),
                    x => x.Insert(1, processor2)
                )
                .GetConfiguration();

            Pipeline.From(processor3).Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void Insert_ShouldPasteProperlyIntoIndexOne_WhenInstanceIsPassedIntoNonEmptyPipeline()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure(
                    x => x.Insert(1, processor2)
                )
                .GetConfiguration();

            processor1.ThenProcessor(processor3).ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor2, processor3);
        }

        [Fact]
        public void Insert_ShouldPasteProperlyIntoIndexTwo_WhenInstanceIsPassedIntoNonEmptyPipeline()
        {
            var processor1 = new TestProcessor();
            var processor2 = new TestProcessor();
            var processor3 = new TestProcessor();

            var configuration = Modification.Configure(
                    x => x.Insert(2, processor2)
                )
                .GetConfiguration();

            processor1.ThenProcessor(processor3).ToPipeline().Modify(configuration)
                .GetProcessors().Should().Equal(processor1, processor3, processor2);
        }
    }
}
