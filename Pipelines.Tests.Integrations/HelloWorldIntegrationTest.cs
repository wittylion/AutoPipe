using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class HelloWorldIntegrationTest
    {
        [Fact]
        public async void Pipeline_Mechanism_Is_Able_To_Produce_A_Simple_Result()
        {
            PipelineRunner runner = new PipelineRunner();

            var arguments = new HelloWorldArguments() { Name = "Sergey" };
            await runner.RunPipeline(new HelloWorldPipeline(), arguments).ConfigureAwait(false);

            arguments.Result.Should().Be("Hello, Sergey!",
                $"we've passed name '{arguments.Name}' to the pipeline, and expect it to be displayed in phrase 'Hello, {arguments.Name}!'");
        }

        [Fact]
        public async void Pipeline_Mechanism_Produces_A_Simple_Check_Before_Returning_A_Result()
        {
            PipelineRunner runner = new PipelineRunner();

            var arguments = new HelloWorldArguments { Name = "   " };
            await runner.RunPipeline(new HelloWorldPipelineWithValidation(), arguments).ConfigureAwait(false);

            arguments.GetMessages(MessageFilter.Errors).Should().ContainSingle(pipelineMessage =>
                pipelineMessage.Message.Equals(HelloWorldPipelineMessages.NameMustBeProvided));
        }
    }

    public class HelloWorldPipeline : IPipeline
    {
        public IEnumerable<IProcessor> GetProcessors()
        {
            return new HelloWorldProcessors[]
            {
                new PutNameIntoThePhrase()
            };
        }
    }

    public class HelloWorldPipelineWithValidation : IPipeline
    {
        public IEnumerable<IProcessor> GetProcessors()
        {
            return new HelloWorldProcessors[]
            {
                new WhenTheNameIsNotProvidedAbortWithErrorMessage(),
                new PutNameIntoThePhrase()
            };
        }
    }

    public static class HelloWorldPipelineMessages
    {
        public static readonly string NameMustBeProvided =
            "To get a proper result of this pipeline, please provide a valuable name.";
    }

    public abstract class HelloWorldProcessors : SafeProcessor<HelloWorldArguments> { }

    public class WhenTheNameIsNotProvidedAbortWithErrorMessage : HelloWorldProcessors
    {
        public override Task SafeExecute(HelloWorldArguments args)
        {
            args.AbortPipelineWithErrorMessage(HelloWorldPipelineMessages.NameMustBeProvided);
            return PipelineTask.CompletedTask;
        }

        public override bool SafeCondition(HelloWorldArguments args)
        {
            return base.SafeCondition(args) && string.IsNullOrWhiteSpace(args.Name);
        }
    }

    public class PutNameIntoThePhrase : HelloWorldProcessors
    {
        public override Task SafeExecute(HelloWorldArguments args)
        {
            args.Result = "Hello, " + args.Name + "!";
            return PipelineTask.CompletedTask;
        }
    }

    public class HelloWorldArguments : Bag
    {
        public string Name { get; set; }
        public string Result { get; set; }
    }
}
