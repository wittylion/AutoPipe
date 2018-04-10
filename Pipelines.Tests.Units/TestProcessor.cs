using System;
using System.Threading.Tasks;

namespace Pipelines.Tests.Units
{
    public class TestProcessor : SafeProcessor<PipelineContext>
    {
        public Action Action { get; }

        public TestProcessor(Action action)
        {
            Action = action;
        }

        public override Task SafeExecute(PipelineContext args)
        {
            return Task.Run(Action);
        }
    }
}