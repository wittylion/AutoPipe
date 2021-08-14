using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public abstract class ExecuteActionForPropertyProcessorConcept<TContext, TProperty> : SafeProcessor<TContext>
        where TContext : Bag
    {
        public override Task SafeExecute(TContext args)
        {
            var propertyName = this.GetPropertyName(args);
            if (!args.ContainsProperty<object>(propertyName))
            {
                return this.MissingPropertyHandler(args);
            }

            if (!args.ContainsProperty(propertyName, out TProperty property))
            {
                return this.WrongPropertyTypeHandler(args, args.GetOrThrow<object>(propertyName));
            }

            return this.PropertyExecution(args, property);
        }

        public virtual Task MissingPropertyHandler(TContext args)
        {
            return PipelineTask.CompletedTask;
        }

        public virtual Task WrongPropertyTypeHandler(TContext args, object property)
        {
            return PipelineTask.CompletedTask;
        }

        public abstract Task PropertyExecution(TContext args, TProperty property);
        public abstract string GetPropertyName(TContext args);
    }

    public abstract class ExecuteActionForPropertyProcessorConcept : ExecuteActionForPropertyProcessorConcept<Bag, object>
    {
    }
}