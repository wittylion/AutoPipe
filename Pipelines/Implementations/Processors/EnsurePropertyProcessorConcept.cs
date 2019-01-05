using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor that simply gets a key of the property
    /// and value to be added then adds key and value to
    /// the PipelineContext if the key doesn't exist.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of the value object that will be added for a given key.
    /// </typeparam>
    public abstract class EnsurePropertyProcessorConcept<TValue> : SafeProcessor
    {
        public override Task SafeExecute(PipelineContext args)
        {
            var name = this.GetName(args);
            var value = this.GetValue(args);

            args.AddOrSkipPropertyIfExists(name, value);

            return PipelineTask.CompletedTask;
        }

        public abstract string GetName(PipelineContext args);
        public abstract TValue GetValue(PipelineContext args);
    }
}