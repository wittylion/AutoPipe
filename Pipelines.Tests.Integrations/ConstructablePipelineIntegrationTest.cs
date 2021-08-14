﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Pipelines.Implementations.Pipelines;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class ConstructablePipelineIntegrationTest
    {
        [Fact]
        public async void When_Using_Action_Method_In_Pipeline_Declaration_It_Will_Be_Executed()
        {
            var context = new Bag();
            await new AddMessagePipeline().Run(context).ConfigureAwait(false);
            context.GetAllMessages().Length
                .Should().Be(1, "because pipeline intended to add a message");
        }

        [Fact]
        public async void When_Using_Iterator_Method_In_Pipeline_Declaration_It_Will_Be_Executed()
        {
            var context = new Bag(new { Messages = ContextValues.Messages });
            await new AddMessagesFromPropertyPipeline().Run(context).ConfigureAwait(false);
            context.GetAllMessages().Select(x => x.Message)
                .Should()
                .BeEquivalentTo(ContextValues.Messages, "because pipeline should add passed messages to the property");
        }

        [Fact]
        public async void When_Using_Iterator_And_Setter_Methods_In_Pipeline_Declaration_They_Will_Be_Executed()
        {
            var context = new Bag();
            await new SetMessagesAndAddFromPropertyPipeline().Run(context).ConfigureAwait(false);
            context.GetAllMessages().Select(x => x.Message)
                .Should()
                .BeEquivalentTo(ContextValues.Messages, "because pipeline should add passed messages to the property");
        }

        public class ContextProperties
        {
            public static readonly string Messages = "Messages";
        }

        public class ContextValues
        {
            public static readonly string[] Messages = { "One", "Two", "Ten", "Five" };
        }

        public class AddMessagePipeline : IPipeline
        {
            public IEnumerable<IProcessor> GetProcessors()
            {
                yield return Processor.From<Bag>(x =>
                    x.AddMessage(nameof(ConstructablePipelineIntegrationTest)));
            }
        }

        public class AddMessagesFromPropertyPipeline : IPipeline
        {
            public IEnumerable<IProcessor> GetProcessors()
            {
                yield return Processor.From<Bag>(b =>
                {
                    var messages = b.Get(ContextProperties.Messages, Array.Empty<string>()).ToList();
                    messages.ForEach((x) => b.AddMessage(x));
                });
            }
        }

        public class SetMessagesAndAddFromPropertyPipeline : IPipeline
        {
            public IEnumerable<IProcessor> GetProcessors()
            {
                yield return Processor.From<Bag>(b => b.SetOrAddProperty(ContextProperties.Messages,
                    ContextValues.Messages));

                yield return new AddMessagesFromPropertyPipeline().ToProcessor();
            }
        }
    }
}