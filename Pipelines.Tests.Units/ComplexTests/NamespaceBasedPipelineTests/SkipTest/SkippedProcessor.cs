using System;
using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SkipTest
{
    [Skip]
    public class SkippedProcessor : SafeProcessor
    {
        public override Task SafeRun(Bag args)
        {
            throw new NotImplementedException();
        }
    }
}
