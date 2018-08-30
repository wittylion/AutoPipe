using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class PipelineContextPropertiesUsageTest
    {
        [Fact]
        public async void When_Using_An_Empty_Context_Simple_Hello_World_Message_Is_Retrieved()
        {
            var context = new PipelineContext();
            await new SetMessage().Execute(context);
            context.GetPropertyValueOrNull<string>(GreeterProperties.Message)
                .Should()
                .Be(GreeterValues.HelloWorld, "because nothing is set in the context");
        }

        [Fact]
        public async void When_Using_A_Context_With_Name_Property_Greeting_Message_Is_Retrieved()
        {
            var name = nameof(When_Using_A_Context_With_Name_Property_Greeting_Message_Is_Retrieved);

            var context = new PipelineContext();
            context.AddOrSkipPropertyIfExists(GreeterProperties.Name, name);

            await new SetMessage().Execute(context);

            context.GetPropertyValueOrNull<string>(GreeterProperties.Message)
                .Should()
                .Be(string.Format(GreeterValues.WelcomeAboard, name), "because name is set in the context");
        }

        [Fact]
        public async void When_Using_An_Object_Constructor_Of_Pipeline_Context_Greeting_Message_Is_Retrieved()
        {
            var name = nameof(When_Using_An_Object_Constructor_Of_Pipeline_Context_Greeting_Message_Is_Retrieved);

            var context = new PipelineContext(new {Name = name});
            await new SetMessage().Execute(context);

            context.GetPropertyValueOrNull<string>(GreeterProperties.Message)
                .Should()
                .Be(string.Format(GreeterValues.WelcomeAboard, name),
                    "because name is set through object constructor in the context");
        }

        [Fact]
        public async void When_Using_An_Intermediate_Processor_Its_Message__Must_Be_Returned()
        {
            var context = new PipelineContext();
            await new DefaultMessageSetter().Execute(context);

            context.GetPropertyValueOrNull<string>(GreeterProperties.Message)
                .Should()
                .Be(GreeterValues.IntermediateMessage, "because intermediate processor should set its own message");
        }

        [Fact]
        public async void
            When_Using_An_Intermediate_Processor_Its_Message__Must_Be_Returned_Even_When_Custom_Message_Is_Set()
        {
            var context = new PipelineContext(new {Message = "DEFAULT"});
            await new DefaultMessageSetter().Execute(context);

            context.GetPropertyValueOrNull<string>(GreeterProperties.Message)
                .Should()
                .Be(GreeterValues.IntermediateMessage, "because intermediate processor should set its own message");
        }

        public class GreeterProperties
        {
            public static readonly string Name = "Name";
            public static readonly string Message = "Message";
        }

        public class GreeterValues
        {
            public static readonly string HelloWorld = "Hello world!";
            public static readonly string WelcomeAboard = "Welcome aboard {0}!";
            public static readonly string IntermediateMessage = "This message is from intermediate processor";
        }

        public class DefaultMessageSetter : SafeProcessor
        {
            public override Task SafeExecute(PipelineContext args)
            {
                args.UpdateOrAddProperty(GreeterProperties.Message, GreeterValues.IntermediateMessage);
                return new SetMessage().Execute(args);
            }
        }

        public class SetMessage : SafeProcessor
        {
            public override Task SafeExecute(PipelineContext args)
            {
                var name = args.GetPropertyValueOrNull<string>(GreeterProperties.Name);

                if (string.IsNullOrWhiteSpace(name))
                {
                    args.AddOrSkipPropertyIfExists(GreeterProperties.Message, GreeterValues.HelloWorld);
                }
                else
                {
                    args.AddOrSkipPropertyIfExists(GreeterProperties.Message,
                        string.Format(GreeterValues.WelcomeAboard, name));
                }

                return PipelineTask.CompletedTask;
            }
        }
    }
}
