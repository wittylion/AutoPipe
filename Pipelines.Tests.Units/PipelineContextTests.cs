using System.Linq;
using FluentAssertions;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineContextTests
    {
        [Fact]
        public void GetMessages_Returns_Information_Messages_When_All_Types_Of_Messages_Are_Added()
        {
            var information = new PipelineMessage(nameof(PipelineContext), MessageType.Information);
            var pipelineContext = new PipelineContextTestObject()
                .WithWarning(nameof(PipelineContext))
                .WithMessage(information)
                .WithError(nameof(PipelineContext));

            pipelineContext.GetMessages(MessageFilter.Informations).Should()
                .HaveCount(1, "only one information message was added")
                .And
                .AllBeEquivalentTo(information, "this message was added to collection");
        }

        [Fact]
        public void AbortMessageWithError_Calls_Abort_Pipeline_Method()
        {
            var pipelineContext = new PipelineContextTestObject();
            pipelineContext.AbortPipelineWithErrorMessage(nameof(PipelineContext));
            pipelineContext.IsAborted.Should().BeTrue("because the method should abort pipeline");
        }

        [Fact]
        public void GetInformationsAndWarnings_Should_Retrieve_Warnings_And_Informations_When_Several_Message_Are_Added()
        {
            var pipelineContext = new PipelineContextTestObject()
                .WithError("Error")
                .WithInformation("Information")
                .WithWarning("Warning");

            pipelineContext.GetInformationsAndWarnings().Should()
                .HaveCount(2, "because three messages were added, where only one of them is error")
                .And
                .Match(collection => collection.All(x => x.MessageType != MessageType.Error), "because collection should contain no Error type");
        }
    }

    public class PipelineContextTestObject : PipelineContext
    {
        public virtual PipelineContextTestObject WithMessage(PipelineMessage message)
        {
            this.AddMessageObject(message);
            return this;
        }

        public virtual PipelineContextTestObject WithMessage(string message)
        {
            this.AddMessage(message);
            return this;
        }

        public virtual PipelineContextTestObject WithMessage(string message, MessageType type)
        {
            this.AddMessage(message, type);
            return this;
        }

        public virtual PipelineContextTestObject WithWarning(string message)
        {
            this.AddWarning(message);
            return this;
        }

        public virtual PipelineContextTestObject WithInformation(string message)
        {
            this.AddInformation(message);
            return this;
        }

        public virtual PipelineContextTestObject WithError(string message)
        {
            this.AddError(message);
            return this;
        }
    }
}
