using System;
using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SkipTest
{
    public class IncludedProcessor : SafeProcessor
    {
        public override Task SafeExecute(PipelineContext args)
        {
            throw new NotImplementedException();
        }
    }
}
