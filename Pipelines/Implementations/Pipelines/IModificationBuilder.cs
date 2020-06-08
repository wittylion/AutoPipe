using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelines.Implementations.Pipelines
{
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

    public class ProcessorMatcherByType : IProcessorMatcher
    {
        public ProcessorMatcherByType(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public bool Matches(IProcessor processor)
        {
            return processor.GetType() == Type;
        }
    }

    public class ProcessorMatcherByInstance : IProcessorMatcher
    {
        public ProcessorMatcherByInstance(IProcessor processor)
        {
            Processor = processor;
        }

        public IProcessor Processor { get; }

        public bool Matches(IProcessor processor)
        {
            return processor == Processor;
        }
    }

    public class A
    {
        public static IProcessorMatcher Processor<TProcessor>()
        {
            return new ProcessorMatcherByType(typeof(TProcessor));
        }

        public static IProcessorMatcher Processor(IProcessor processor)
        {
            return new ProcessorMatcherByInstance(processor);
        }
    }

    public class OriginalModificationConfiguration : IModificationConfiguration
    {
        public Dictionary<IProcessorMatcher, Queue<IProcessor>> Before { get; } = new Dictionary<IProcessorMatcher, Queue<IProcessor>>();
        public Dictionary<IProcessorMatcher, IProcessor> Instead { get; } = new Dictionary<IProcessorMatcher, IProcessor>();
        public Dictionary<IProcessorMatcher, Queue<IProcessor>> After { get; } = new Dictionary<IProcessorMatcher, Queue<IProcessor>>();

        public IEnumerable<IProcessor> GetModifications(IProcessor processor)
        {
            var processorType = processor.GetType();
            if (processor.IsNull())
            {
                yield break;
            }

            foreach (var matcher in Before.Keys.Where(m => m.Matches(processor)))
            {
                foreach (var preProcessor in Before[matcher])
                {
                    yield return preProcessor;
                }
            }

            var insteadMatcher = Instead.Keys.FirstOrDefault(m => m.Matches(processor));
            if (insteadMatcher != null)
            {
                yield return Instead[insteadMatcher];
            }
            else
            {
                yield return processor;
            }


            foreach (var matcher in After.Keys.Where(m => m.Matches(processor)))
            {
                foreach (var preProcessor in After[matcher])
                {
                    yield return preProcessor;
                }
            }
        }
    }
}
