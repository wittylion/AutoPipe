using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public abstract class EnsurePropertyProcessorConcept<TValue> : SafeProcessor
    {
        public override Task SafeExecute(PipelineContext args)
        {
            var name = this.GetName();
            var value = this.GetValue();

            args.AddOrSkipPropertyIfExists(name, value);

            return PipelineTask.CompletedTask;
        }

        public abstract string GetName();
        public abstract TValue GetValue();
    }
}