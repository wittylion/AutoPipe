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
            Runner runner = new Runner();

            var arguments = new HelloWorldArguments() { Name = "Sergey" };
            await new HelloWorldPipeline().Run(arguments, runner).ConfigureAwait(false);

            arguments.StringResult().Should().Be("Hello, Sergey!",
                $"we've passed name '{arguments.Name}' to the pipeline, and expect it to be displayed in phrase 'Hello, {arguments.Name}!'");
        }

        [Fact]
        public async void Pipeline_Mechanism_Produces_A_Simple_Check_Before_Returning_A_Result()
        {
            Runner runner = new Runner();

            var arguments = new HelloWorldArguments { Name = "   " };
            await new HelloWorldPipelineWithValidation().Run(arguments, runner).ConfigureAwait(false);

            arguments.MessageObjects(MessageFilter.Error).Should().ContainSingle(pipelineMessage =>
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
                new WhenTheNameIsNotProvidedEndWithErrorMessage(),
                new PutNameIntoThePhrase()
            };
        }
    }

    public static class HelloWorldPipelineMessages
    {
        public static readonly string NameMustBeProvided =
            "To get a proper result of this pipeline, please provide a valuable name.";
    }

    public abstract class HelloWorldProcessors : SafeProcessor { }

    public class WhenTheNameIsNotProvidedEndWithErrorMessage : HelloWorldProcessors
    {
        public override Task SafeRun(Bag args)
        {
            args.ErrorEnd(HelloWorldPipelineMessages.NameMustBeProvided);
            return PipelineTask.CompletedTask;
        }

        public override bool SafeCondition(Bag args)
        {
            return base.SafeCondition(args) && string.IsNullOrWhiteSpace(args.String("Name"));
        }
    }

    public class PutNameIntoThePhrase : HelloWorldProcessors
    {
        public override Task SafeRun(Bag args)
        {
            args["Result"] = "Hello, " + args.String("Name") + "!";
            return PipelineTask.CompletedTask;
        }
    }

    public class HelloWorldArguments : Bag
    {
        public string Name { get => this.String("Name"); set => this["Name"] = value; }
    }
}
