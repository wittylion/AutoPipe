using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.NonProcessorClassTest
{
    public class NonProcessorClassTests
    {
        [Fact]
        public void Executing_Getter_Class_With_AutoProcessor_Should_Return_Message()
        {
            new AutoProcessor(new Getter()).RunSync()
                .String("message")
                .Should()
                .Be(Getter.Name);
        }
    }
}
