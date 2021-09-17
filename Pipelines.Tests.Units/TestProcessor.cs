using System;
using System.Threading.Tasks;

namespace AutoPipe.Tests.Units
{
    public class TestProcessor : SafeProcessor
    {
        public Action Action { get; }

        public TestProcessor() : this(() => { })
        {
        }

        public TestProcessor(Action action)
        {
            Action = action;
        }

        public override Task SafeRun(Bag args)
        {
            return Task.Run(Action);
        }
    }
}