using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public abstract class ExecuteActionForPropertyProcessorConcept<TContext, TProperty> : SafeProcessor<TContext>
        where TContext : PipelineContext
    {
        public override Task SafeExecute(TContext args)
        {
            var propertyName = this.GetPropertyName(args);
            var propertyHolder = args.GetPropertyObjectOrNull(propertyName);
            if (!propertyHolder.HasValue)
            {
                return this.MissingPropertyHandler(args);
            }

            if (!(propertyHolder.Value.Value is TProperty propertyValue))
            {
                return this.WrongPropertyTypeHandler(args, propertyHolder.Value);
            }

            return this.PropertyExecution(args, propertyValue);
        }

        public virtual Task MissingPropertyHandler(TContext args)
        {
            return PipelineTask.CompletedTask;
        }

        public virtual Task WrongPropertyTypeHandler(TContext args, PipelineProperty property)
        {
            return PipelineTask.CompletedTask;
        }

        public abstract Task PropertyExecution(TContext args, TProperty property);
        public abstract string GetPropertyName(TContext args);
    }

    public abstract class ExecuteActionForPropertyProcessorConcept : ExecuteActionForPropertyProcessorConcept<PipelineContext, object>
    {
    }
}