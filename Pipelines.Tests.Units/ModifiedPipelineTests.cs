using FluentAssertions;
using Moq;
using System;
using Xunit;
using AutoPipe.Modifications;

namespace AutoPipe.Tests.Units
{
    public class ModifiedPipelineTests
    {
        [Fact]
        public void ModifiedPipeline_ShouldThrowArgumentException_WhenNullIsPassedToOriginalPipeline()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new ModifiedPipeline(null, new Mock<IModificationConfiguration>().Object)).Message.Should().Contain(ModifiedPipeline.PipelineMustBeSpecified);
        }

        [Fact]
        public void ModifiedPipeline_ShouldThrowArgumentException_WhenNullIsPassedToConfiguration()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new ModifiedPipeline(new Mock<IPipeline>().Object, null)).Message.Should().Contain(ModifiedPipeline.ConfigurationMustBeSpecified);
        }

        [Fact]
        public void ModifiedPipeline_ShouldReturnNewProcessorsInsteadOfExisting_WhenSubstitutionDefinedInConfiguration()
        {
            IProcessor processor1 = new Mock<IProcessor>().Object;
            IProcessor processor2 = new Mock<IProcessor>().Object;
            IModificationConfiguration configuration = new SubstituteProcessorModification(processor1.GetMatcher(), processor2.ToAnArray());

            new ModifiedPipeline(processor1.ToAnArray().ToPipeline(), configuration).GetProcessors().Should().ContainSingle(x => x == processor2);
        }

        [Fact]
        public void ModifiedPipeline_ShouldReturnNewProcessorsInsteadOfExisting_WhenSubstitutionsDefinedInConfiguration()
        {
            IProcessor processor1 = new Mock<IProcessor>().Object;
            IProcessor processor2 = new Mock<IProcessor>().Object;
            IProcessor processor3 = new Mock<IProcessor>().Object;
            IModificationConfiguration configuration =
                new SubstituteProcessorModification(processor1.GetMatcher(), processor2.ThenProcessor(processor3));

            new ModifiedPipeline(processor1.ToAnArray().ToPipeline(), configuration)
                .GetProcessors().Should().Equal(processor2, processor3);
        }

        [Fact]
        public void ModifiedPipeline_ShouldReturnNewProcessorsInsteadOfExistingFew_WhenSubstitutionsDefinedInConfiguration()
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

            new ModifiedPipeline(processor1.ThenProcessor(processor2).ToPipeline(), configuration)
                .GetProcessors().Should().Equal(processor3, processor3, processor4, processor3);
        }
    }
}
