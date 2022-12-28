using System;
using System.Threading.Tasks;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SkipTest
{
    [Skip]
    public class SkippedProcessor : Processor
    {
        public override Task SafeRun(Bag args)
        {
            throw new NotImplementedException();
        }
    }
}
