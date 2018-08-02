using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Processors
{
    public abstract class ExecuteForEachElementInPropertyProcessorConcept : SafeProcessor
    {
        public override async Task SafeExecute(PipelineContext args)
        {
            var propertyName = this.GetPropertyName();
            var property = args.GetPropertyValueOrNull<IEnumerable>(propertyName);
            if (property.HasValue())
            {
                foreach (var element in property)
                {
                    await ElementExecution(element);
                }
            }
        }

        public abstract Task ElementExecution(object element);
        public abstract string GetPropertyName();
    }

    public abstract class ExecuteForEachElementInPropertyProcessorConcept<TElement> : SafeProcessor
    {
        public override async Task SafeExecute(PipelineContext args)
        {
            var propertyName = this.GetPropertyName();
            var property = args.GetPropertyValueOrNull<IEnumerable<TElement>>(propertyName);
            if (property.HasValue())
            {
                foreach (var element in property)
                {
                    await ElementExecution(element);
                }
            }
        }

        public abstract Task ElementExecution(TElement element);
        public abstract string GetPropertyName();
    }
}