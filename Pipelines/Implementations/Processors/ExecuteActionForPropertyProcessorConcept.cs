using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Processors
{
    public abstract class ExecuteActionForPropertyProcessorConcept<TContext, TProperty> : SafeProcessor<TContext>
        where TContext : PipelineContext
    {
        public override Task SafeExecute(TContext args)
        {
            var propertyName = this.GetPropertyName(args);
            if (!args.ContainsProperty(propertyName))
            {
                return PipelineTask.CompletedTask;
            }

            var property = args.GetPropertyValueOrDefault(propertyName, default(TProperty));
            return this.PropertyExecution(args, property);
        }

        public abstract Task PropertyExecution(TContext args, TProperty property);
        public abstract string GetPropertyName(TContext args);
    }

    public abstract class ExecuteActionForPropertyProcessorConcept : SafeProcessor<PipelineContext>
    {
        public override Task SafeExecute(PipelineContext args)
        {
            var propertyName = this.GetPropertyName(args);
            if (!args.ContainsProperty(propertyName))
            {
                return PipelineTask.CompletedTask;
            }

            var property = args.GetPropertyValueOrNull<object>(propertyName);
            return this.PropertyExecution(args, property);
        }

        public abstract Task PropertyExecution(PipelineContext args, object property);
        public abstract string GetPropertyName(PipelineContext args);
    }
}