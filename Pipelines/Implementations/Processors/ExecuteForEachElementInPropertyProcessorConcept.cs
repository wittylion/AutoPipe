using System.Collections.Generic;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Processors
{
    public abstract class ExecuteForEachElementInPropertyProcessorConcept<TContext, TElement> : SafeProcessor<TContext> where TContext : PipelineContext
    {
        public override async Task SafeExecute(TContext args)
        {
            var propertyName = this.GetPropertyName(args);
            var property = args.GetPropertyValueOrNull<IEnumerable<TElement>>(propertyName);
            if (property.HasValue())
            {
                await this.CollectionExecution(args, property);
            }
        }

        public virtual async Task CollectionExecution(TContext args, IEnumerable<TElement> collection)
        {
            foreach (var element in collection)
            {
                await ElementExecution(args, element);
            }
        }

        public abstract Task ElementExecution(TContext args, TElement element);
        public abstract string GetPropertyName(TContext args);
    }
}