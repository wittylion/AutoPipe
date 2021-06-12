using System;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.ExtensionMethods;
using Pipelines.Implementations.Contexts;
using Pipelines.Implementations.Pipelines;
using Pipelines.Implementations.Processors;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class TransformPropertyProcessorTests
    {
        [Fact]
        public void Execute_Should_Transform_String_And_Put_New_String_Value_Into_New_Property()
        {
            var messagePattern = "Hello, {0}!";
            var value = nameof(Execute_Should_Transform_String_And_Put_New_String_Value_Into_New_Property);
            Func<string, string> fillPattern = (pattern) => string.Format(pattern, value);
            var processor = new TransformPropertyProcessor<Bag, string, string>("message", fillPattern, "newMessage");

            var properties = new { Message =  messagePattern };
            var context = Bag.Create(properties);
            PredefinedPipeline.FromProcessors(processor).RunSync(context);

            var actualResult = context.GetPropertyValueOrNull<string>("newMessage");
            var expectedResult = string.Format(messagePattern, value);

            actualResult.Should().Be(expectedResult,
                "transform property processor should use the function, that fills in pattern");
        }

        [Fact]
        public void Execute_Should_Transform_String_With_Context_Value_And_Put_New_String_Value_Into_New_Property()
        {
            var messagePattern = "Hello, {0}!";
            var value = nameof(Execute_Should_Transform_String_With_Context_Value_And_Put_New_String_Value_Into_New_Property);
            Func<Bag, string, string> fillPattern = (ctx, pattern) => string.Format(pattern, ctx.GetPropertyValueOrNull<string>("name"));
            var processor = new TransformPropertyProcessor<Bag, string, string>("message", fillPattern, "newMessage");

            var properties = new { Message = messagePattern, Name = value };
            var context = Bag.Create(properties);

            PredefinedPipeline.FromProcessors(processor).RunSync(context);

            var actualResult = context.GetPropertyValueOrNull<string>("newMessage");
            var expectedResult = string.Format(messagePattern, value);

            actualResult.Should().Be(expectedResult,
                "transform property processor should use the function, that fills in pattern");
        }

        [Fact]
        public void Execute_Should_Transform_String_And_Put_New_Integer_Value_Into_New_Property()
        {
            var integer = Int32.MaxValue;
            var message = integer.ToString();
            Func<string, int> transform = int.Parse;
            var processor = new TransformPropertyProcessor<Bag, string, int>("numString", transform, "integer");

            var properties = new { NumString = message };
            var context = Bag.Create(properties);
            PredefinedPipeline.FromProcessors(processor).RunSync(context);

            var actualResult = context.GetPropertyValueOrDefault("integer", 0);
            actualResult.Should().Be(integer,
                "transform property processor should use the function, that parses a number");
        }

        [Fact]
        public void Execute_Should_Transform_String_And_Replace_String_With_New_Integer_Value()
        {
            var integer = Int32.MaxValue;
            var message = integer.ToString();
            Func<string, int> transform = int.Parse;
            var processor = new TransformPropertyProcessor<Bag, string, int>("integer", transform);

            var properties = new { Integer = message };
            var context = Bag.Create(properties);
            PredefinedPipeline.FromProcessors(processor).RunSync(context);

            var actualResult = context.GetPropertyValueOrDefault("integer", 0);
            actualResult.Should().Be(integer,
                "transform property processor should use the function, that parses a number");
        }
    }
}