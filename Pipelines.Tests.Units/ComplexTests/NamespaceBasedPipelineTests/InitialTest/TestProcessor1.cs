using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [Order(1)]
    public class TestProcessor1 : IProcessor
    {
        public Task Run(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}