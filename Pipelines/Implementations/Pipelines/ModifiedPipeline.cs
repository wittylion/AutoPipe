using Pipelines.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    public class ModifiedPipeline : IPipeline
    {
        public static readonly string PipelineMustBeSpecified = "You have to specify an original pipeline to be modified in ModifiedPipeline.";
        public static readonly string ConfigurationMustBeSpecified = "You have to specify a configuration to modifiy pipeline in ModifiedPipeline.";
        public ModifiedPipeline(IPipeline originalPipeline, IModificationConfiguration configuration)
        {
            OriginalPipeline = originalPipeline ?? throw new ArgumentNullException(PipelineMustBeSpecified);
            Configuration = configuration ?? throw new ArgumentNullException(ConfigurationMustBeSpecified);
        }

        public IPipeline OriginalPipeline { get; }
        public IModificationConfiguration Configuration { get; }

        public IEnumerable<IProcessor> GetProcessors()
        {
            foreach (var processor in OriginalPipeline.GetProcessors())
            {
                foreach(var substitute in Configuration.GetModifications(processor))
                {
                    yield return substitute;
                }
            }
        }
    }

    public interface IModificationConfiguration 
    {
        IEnumerable<IProcessor> GetModifications(IProcessor processorType);
    }

    public interface IModificationBuilder
    {
        IModificationBuilder Before<TProcessorOriginal, TProcessorBefore>() where TProcessorBefore : new();
        IModificationBuilder Before<TProcessorOriginal>(IProcessor before);
        IModificationBuilder Before(Type original, IProcessor before);
        IModificationBuilder Before(IProcessor original, IProcessor before);
        IModificationBuilder Before(IProcessor original, Func<IProcessor> before);

        IModificationBuilder After<TProcessorOriginal, TProcessorAfter>() where TProcessorAfter : new();
        IModificationBuilder After<TProcessorOriginal>(IProcessor after);
        IModificationBuilder After(Type original, IProcessor after);
        IModificationBuilder After(IProcessor original, IProcessor after);
        IModificationBuilder After(IProcessor original, Func<IProcessor> after);

        IModificationBuilder Instead<TProcessorOriginal, TProcessorReplacement>() where TProcessorReplacement : new();
        IModificationBuilder Instead<TProcessorOriginal>(IProcessor replacement);
        IModificationBuilder Instead(Type original, IProcessor replacement);
        IModificationBuilder Instead(IProcessor original, IProcessor replacement);
        IModificationBuilder Instead(IProcessor original, Func<IProcessor> replacement);

        IModificationConfiguration GetConfiguration();
    }

    public interface IProcessorMatcher
    {
        bool Matches(IProcessor processor);
    }

    public class OriginalModificationConfiguration : IModificationConfiguration
    {
        public Dictionary<Type, Queue<IProcessor>> Before { get; } = new Dictionary<Type, Queue<IProcessor>>();
        public Dictionary<Type, IProcessor> Instead { get; } = new Dictionary<Type, IProcessor>();
        public Dictionary<Type, Queue<IProcessor>> After { get; } = new Dictionary<Type, Queue<IProcessor>>();

        public IEnumerable<IProcessor> GetModifications(IProcessor processor)
        {
            var processorType = processor.GetType();
            if (processor.IsNull())
            {
                yield break;
            }

            if (Before.ContainsKey(processorType))
            {
                foreach (var preProcessor in Before[processorType])
                {
                    yield return preProcessor;
                }
            }

            if (Instead.ContainsKey(processorType))
            {
                yield return Instead[processorType];
            }
            else
            {
                yield return processor;
            }

            if (After.ContainsKey(processorType))
            {
                foreach (var preProcessor in Before[processorType])
                {
                    yield return preProcessor;
                }
            }
        }
    }
}
