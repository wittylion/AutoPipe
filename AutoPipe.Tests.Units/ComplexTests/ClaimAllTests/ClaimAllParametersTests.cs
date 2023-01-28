using AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.NonBlockingScenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests
{
    public class ClaimAllParametersTests
    {
        private readonly ITestOutputHelper output;

        public ClaimAllParametersTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [MemberData(nameof(ClaimAllParametersDataSets.GetBlockingExecutionProcessors), MemberType = typeof(ClaimAllParametersDataSets))]
        public async void Processor_Should_Claim_All_Properties__PositiveScenario1(object processor)
        {
            var bag = new Bag(debug: true) { ["message"] = "HelloWorld" };
            await AutoProcessor.From(processor).Run(bag);
            bag.Bool("valid").Should().BeTrue(Settings.Explanation, "pipeline was ended", bag.Summary());
        }

        [Theory]
        [MemberData(nameof(ClaimAllParametersDataSets.GetBlockingExecutionProcessors), MemberType = typeof(ClaimAllParametersDataSets))]
        public async void Processor_Should_Claim_All_Properties__NegativeScenario1(object processor)
        {
            var bag = new Bag(debug: true);
            await AutoProcessor.From(processor).Run(bag);
            bag.DoesNotContain("valid").Should().BeTrue(Settings.Explanation, "pipeline was ended", bag.Summary());
        }

        [Theory]
        [MemberData(nameof(ClaimAllParametersDataSets.GetBlockingExecutionProcessors), MemberType = typeof(ClaimAllParametersDataSets))]
        public async void Processor_Should_Claim_All_Properties__NegativeScenario2(object processor)
        {
            var bag = new Bag(debug: true) { ["message"] = int.MaxValue };
            await AutoProcessor.From(processor).Run(bag);
            bag.DoesNotContain("valid").Should().BeTrue(Settings.Explanation, "pipeline was ended", bag.Summary());
        }

        [Fact]
        public async void RunClassRequireRunMethodNotProcessor_Should_Claim_All_Properties__PositiveScenario1()
        {
            var bag = new Bag() { ["message"] = "HelloWorld" };
            await AutoProcessor.From<RunClassRequireRunMethodNotProcessor>().Run(bag);
            bag.Bool("valid").Should().BeTrue();
        }

        [Fact]
        public async void RunClassRequireRunMethodNotProcessor_Should_Claim_All_Properties__NegativeScenario1()
        {
            var bag = new Bag(debug: true);
            await AutoProcessor.From<RunClassRequireRunMethodNotProcessor>().Run(bag);
            bag.Bool("valid").Should().BeFalse(Settings.Explanation, "pipeline was ended", bag.Summary());
        }

        [Fact]
        public async void RunClassRequireRunMethodNotProcessor_Should_Claim_All_Properties__NegativeScenario2()
        {
            var bag = new Bag(debug: true) { ["message"] = int.MaxValue };
            await AutoProcessor.From<RunClassRequireRunMethodNotProcessor>().Run(bag);
            bag.Bool("valid").Should().BeFalse(Settings.Explanation, "pipeline was ended", bag.Summary());
        }
    }
}
