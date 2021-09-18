using FluentAssertions;
using System;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class AkaAttributeTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_The_First_Parameter_Is_Null()
        {
            Assert.Throws<ArgumentException>(() => new AkaAttribute(null))
                .Message.Should().StartWith(AkaAttribute.NameParameterIsEmpty);
        }

        [Fact]
        public void Constructor_Should_Throw_When_The_First_Parameter_Is_White_Space()
        {
            Assert.Throws<ArgumentException>(() => new AkaAttribute(" "))
                .Message.Should().StartWith(AkaAttribute.NameParameterIsEmpty);
        }

        [Fact]
        public void Constructor_Should_Throw_When_The_Second_Parameter_Is_White_Space()
        {
            Assert.Throws<ArgumentException>(() => new AkaAttribute("First", " "))
                .Message.Should().StartWith(AkaAttribute.AliasIsEmpty.FormatWith(2));
        }

        [Fact]
        public void Constructor_Should_Throw_When_The_Third_Parameter_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() => new AkaAttribute("First", "Second", ""))
                .Message.Should().StartWith(AkaAttribute.AliasIsEmpty.FormatWith(3));
        }
    }
}
