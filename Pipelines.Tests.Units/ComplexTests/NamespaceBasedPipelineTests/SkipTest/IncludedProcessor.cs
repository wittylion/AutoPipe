using System;
using System.Threading.Tasks;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SkipTest
{
    public class IncludedProcessor : SafeProcessor
    {
        public override Task SafeRun(Bag args)
        {
            throw new NotImplementedException();
        }
    }
}
