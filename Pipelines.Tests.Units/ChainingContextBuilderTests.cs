﻿using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class ChainingContextBuilderTests
    {
        [Fact]
        public async void RunWith_Should_Run_The_Passed_Pipeline_With_Preset_Properties()
        {
            var value = await Bag.Create()
                .Use("one", 1)
                .Use("two", 2)
                .Use("string", "aqwe")
                .Run(
                    Pipeline.From(
                        Processor.From<Bag>(
                            (ctx) => ctx.Set("string", ctx.Get("one", 0) + ctx.Get("two", 0)))),
                    PipelineRunner.StaticInstance).ContinueWith(a => a.Result.Get("string", 0));

            value.Should().Be(3);
        }

        [Fact]
        public async void RunWith_Should_Run_The_Passed_Pipeline()
        {
            var context = await Bag.Create()
                .Run(
                    Pipeline.From(
                        new EnsureName(),
                        new EnsureMessage(),
                        new HelloMessageNameReplacer()
                    ),
                    PipelineRunner.StaticInstance);

            context.Get("message", "")
                .Should()
                .Be("Hello, Serge!");
        }

        [Fact]
        public async void UseMethod_Should_Set_A_Template_That_Will_Not_Be_Overriden()
        {
            var context = await Bag.Create()
                .Use("message", "Hola, {name}!")
                .Run(
                    Pipeline.From(
                        new EnsureMessage(),
                        new HelloMessageNameReplacer()
                    ),
                    PipelineRunner.StaticInstance);

            context.Get("message", "")
                .Should()
                .Be("Hola, stranger!");
        }

        [Fact]
        public async void UseMethod_Should_Set_Up_A_Name_That_Will_Not_Be_Overriden()
        {
            var context = await Bag.Create()
                .Use("name", "Bob")
                .Run(
                    Pipeline.From(
                        new EnsureName(),
                        new EnsureMessage(),
                        new HelloMessageNameReplacer()
                    ),
                    PipelineRunner.StaticInstance);

            context.Get("message", "")
                .Should()
                .Be("Hello, Bob!");
        }


        [Fact]
        public async void UseMethod_Should_Set_A_Name_That_Will_Be_Set_In_QueryContext_Result()
        {
            var context = await Bag.Create()
                .Use("name", "Bob")
                .Run(
                    Pipeline.From(
                        new EnsureName(),
                        new EnsureMessage(),
                        new HelloMessageNameToResultReplacer()
                    ),
                    PipelineRunner.StaticInstance);

            context.StringResultOrEmpty()
                .Should()
                .Be("Hello, Bob!");
        }

        public class EnsureName : SafeProcessor<Bag>
        {
            public override Task SafeRun(Bag args)
            {
                args.ApplyProperty("name", ctx => "Serge", PropertyModificator.SkipIfExists);
                return Done;
            }
        }

        public class HelloMessageNameReplacer : SafeProcessor<Bag>
        {
            public override Task SafeRun(Bag args)
            {
                args.TransformProperty(
                    "message",
                    (Bag ctx, string val) => val.Replace("{name}", ctx.Get("name", "stranger")),
                    PropertyModificator.UpdateValue
                );

                return Done;
            }
        }

        public class HelloMessageNameToResultReplacer : SafeProcessor<Bag>
        {
            public override Task SafeRun(Bag args)
            {
                args.IfHasProperty(
                    "message",
                    SetResult
                );

                return Done;
            }

            public virtual void SetResult(Bag ctx)
            {
                var message = ctx.GetOrThrow<string>("message");
                var name = ctx.Get("name", "stranger");
                var result = message.Replace("{name}", name);
                ctx.SetResultWithInformation(result, "The message has been set with name token replaced.");
            }
        }

        public class EnsureMessage : SafeProcessor<Bag>
        {
            public override Task SafeRun(Bag args)
            {
                args.ApplyProperty("message", ctx => "Hello, {name}!", PropertyModificator.SkipIfExists);
                return Done;
            }
        }
    }
}