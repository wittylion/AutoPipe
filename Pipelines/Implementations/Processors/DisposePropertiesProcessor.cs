using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Processors
{
    public class DisposePropertiesProcessor : DisposeProcessorConcept<PipelineContext>
    {
        public static readonly string PropertyNamesNullException = "The property names collection must be specified, so the IDisposables will be released correctly.";
        public string[] PropertyNames { get; }

        public DisposePropertiesProcessor(params string[] propertyNames)
        {
            PropertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames),
                                DisposePropertiesProcessor.PropertyNamesNullException);
        }

        public override IEnumerable<IDisposable> GetDisposables(PipelineContext arguments)
        {
            foreach (var propertyName in PropertyNames)
            {
                yield return arguments.GetPropertyValueOrNull<IDisposable>(propertyName);
            }
        }
    }
}