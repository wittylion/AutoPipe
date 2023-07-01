using FluentAssertions;
using System.Reflection;
using Xunit;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.MarkedRunAllTest
{
    public class MarkedRunAllTests
    {
        [Fact]
        public void Executing_This_Namespace_Should_Execute_A_Class_Marked_With_RunAll()
        {
            new NamespacePipeline().RunSync().GetResult<object>(fallbackValue: null).Should().NotBeNull();
        }

        [Fact]
        public void Deriving_From_AutoProcessor_And_Marking_Class_With_RunAll_Should_Not_Contain_AutoProcessor_Methods()
        {
            new ThisOnIsDerivedFromAutoProcessor().Methods.Should().HaveCount(1);
        }


        [Fact]
        public void Deriving_From_AutoProcessor_And_Marking_Class_With_RunAll_And_Skipping_Method_Should_Not_Contain_Skipped_Method()
        {
            new ThisOnIsDerivedFromAutoProcessorAndContainsMethodToSkip().Methods.Should().HaveCount(1);
        }
    }

    [RunAll]
    public class ThisOnIsDerivedFromAutoProcessorAndContainsMethodToSkip : AutoProcessor
    {
        public object GetResult()
        {
            return new object();
        }

        [Skip]
        public object GetBadResult()
        {
            throw new System.Exception();
        }
    }

    [RunAll]
    public class ThisOnIsDerivedFromAutoProcessor : AutoProcessor
    {
        public object GetResult()
        {
            return new object();
        }
    }

    [RunAll]
    public class ThisOneIsMarkedRunAll
    {
        public object GetResult()
        {
            return new object();
        }
    }
}
