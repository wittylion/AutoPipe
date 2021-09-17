﻿using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class SafeProcessorTests
    {
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Pipeline_Context_Has_Ended_Parameter_Set_To_True()
        {
            var reachedExecution = false;
            var args = new Bag()
            {
                Ended = true
            };
            var processor = new TestProcessor(() => reachedExecution = true);
            await processor.Run(args).ConfigureAwait(false);
            reachedExecution.Should().BeFalse("pipeline was ended");
        }

        [Fact]
        public async Task Safe_Execution_Is_Reached_When_Pipeline_Context_Has_Ended_Parameter_Set_To_False()
        {
            var reachedExecution = false;
            var processor = new TestProcessor(() => reachedExecution = true);
            await Bag.Create().Run(processor).ConfigureAwait(false);
            reachedExecution.Should().BeTrue("pipeline was not ended");
        }
    }
}