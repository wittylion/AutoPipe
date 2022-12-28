using System.Threading.Tasks;

namespace AutoPipe.Tests.Units
{
    public class TestProcessor : Processor
    {
        public override Task SafeRun(Bag bag)
        {
            return PipelineTask.CompletedTask;
        }
    }
}