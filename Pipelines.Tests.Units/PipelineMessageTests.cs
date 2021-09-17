using System;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class PipelineMessageTests
    {
        [Fact]
        public void PipelineMessage_Constructor_Should_Throw_An_Exception_If_String_Is_Empty()
        {
            Assert.Throws<ArgumentException>(nameof(PipelineMessage.Message).ToLower(), () => new PipelineMessage(string.Empty, MessageType.Information));
        }
    }
}
