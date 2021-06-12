using System;
using System.Threading.Tasks;

namespace Pipelines.Tests.Units
{
    public class TestProcessor : SafeProcessor<Bag>
    {
        public Action Action { get; }

        public TestProcessor() : this(() => { })
        {
        }

        public TestProcessor(Action action)
        {
            Action = action;
        }

        public override Task SafeExecute(Bag args)
        {
            return Task.Run(Action);
        }
    }
}