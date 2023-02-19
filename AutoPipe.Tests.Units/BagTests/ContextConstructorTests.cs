using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace AutoPipe.Tests.Units.BagTests
{
    public class ContextConstructorTests
    {
        [Fact]
        public void PipelineContext_When_Created_From_Dictionary_Contains_All_Expected_Properties()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["1"] = nameof(ContextConstructorTests),
                ["2"] = nameof(PipelineContext_When_Created_From_Dictionary_Contains_All_Expected_Properties)
            };
            var context = Bag.CreateFromDictionary(dictionary);

            dictionary.Should().BeEquivalentTo(context);
        }
    }
}