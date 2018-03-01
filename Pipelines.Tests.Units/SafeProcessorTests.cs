using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class SafeProcessorTests
    {
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_There_Is_An_Incorrect_Type()
        {
            var reachedSafeExecution = false;
            IProcessor processor = new StringSafeProcessor(() => reachedSafeExecution = true);
            await processor.Execute(false);
            Assert.False(reachedSafeExecution);
        }

        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Type_Is_Correct()
        {
            var reachedSafeExecution = false;
            IProcessor processor = new StringSafeProcessor(() => reachedSafeExecution = true);
            await processor.Execute(string.Empty);
            Assert.True(reachedSafeExecution);
        }
        
        [Fact]
        public async Task Safe_Execution_Is_Not_Reached_When_Safe_Condition_Returns_False()
        {
            var reachedSafeExecution = false;
            IProcessor processor = new StringSafeProcessor(() => reachedSafeExecution = true, s => false);
            await processor.Execute(string.Empty);
            Assert.False(reachedSafeExecution);
        }
    }

    public class StringSafeProcessor : SafeProcessor<string>
    {
        public Action Action { get; }
        public Maybe<Predicate<string>> SafeCheck { get; }

        public StringSafeProcessor(Action action)
        {
            Action = action;
        }

        public StringSafeProcessor(Action action, Predicate<string> safeCheck)
        {
            Action = action;
            SafeCheck = safeCheck;
        }

        public override bool SafeCondition(string args)
        {
            return base.SafeCondition(args) && SafeCheck.Unwrap(check => check(args), defaultValue: true);
        }

        public override Task SafeExecute(string args)
        {
            return Task.Run(Action);
        }
    }
}