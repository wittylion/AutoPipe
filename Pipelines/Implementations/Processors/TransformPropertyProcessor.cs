using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public class TransformPropertyProcessor<TContext, TProperty, TNewProperty> 
        : ExecuteActionForPropertyProcessorConcept<TContext, TProperty> where TContext : PipelineContext
    {
        public string PropertyToTransform { get; }
        public Func<TContext, TProperty, TNewProperty> TransformFunction { get; }
        public string TransformToProperty { get; }

        public TransformPropertyProcessor(string propertyToTransform,
            string transformToProperty, Func<TContext, TProperty, TNewProperty> transformFunction) : this(propertyToTransform, transformFunction, transformToProperty)
        {
        }

        public TransformPropertyProcessor(string propertyToTransform,
            Func<TContext, TProperty, TNewProperty> transformFunction) : this(propertyToTransform, transformFunction, propertyToTransform)
        {
        }

        public TransformPropertyProcessor(string propertyToTransform, string transformToProperty,
            Func<TProperty, TNewProperty> transformFunction)
            : this(propertyToTransform, transformFunction, transformToProperty)
        {
        }

        public TransformPropertyProcessor(string propertyToTransform,
            Func<TProperty, TNewProperty> transformFunction)
            : this(propertyToTransform, transformFunction, propertyToTransform)
        {
        }

        public TransformPropertyProcessor(string propertyToTransform, Func<TProperty, TNewProperty> transformFunction,
            string transformToProperty)
            : this(propertyToTransform, (context, property) => transformFunction(property), transformToProperty)
        {
        }

        public TransformPropertyProcessor(string propertyToTransform, Func<TContext, TProperty, TNewProperty> transformFunction, string transformToProperty)
        {
            PropertyToTransform = propertyToTransform;
            TransformFunction = transformFunction;
            TransformToProperty = transformToProperty;
        }

        public override Task PropertyExecution(TContext args, TProperty property)
        {
            var newValue = this.TransformFunction(args, property);
            args.SetOrAddProperty(TransformToProperty, newValue);
            return PipelineTask.CompletedTask;
        }

        public override string GetPropertyName(TContext args)
        {
            return this.PropertyToTransform;
        }
    }
}