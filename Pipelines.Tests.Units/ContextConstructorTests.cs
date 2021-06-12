using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.Implementations.Contexts;
using Xunit;

namespace Pipelines.Tests.Units
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

            context.GetAllPropertyObjects().ToDictionary(x => x.Name, x => x.Value).Should().BeEquivalentTo(dictionary);
        }
    }
}