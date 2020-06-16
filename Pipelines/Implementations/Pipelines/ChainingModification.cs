using Pipelines.ExtensionMethods;
using Pipelines.Implementations.Processors;
using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class ChainingModification
    {
        protected LinkedList<IModificationConfiguration> configurations = new LinkedList<IModificationConfiguration>();

        public ChainingModification After<TProcessorOriginal, TProcessorAfter>() where TProcessorAfter : IProcessor, new() where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>(new TProcessorAfter());
        }

        public ChainingModification After<TProcessorOriginal>(IProcessor successor) where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>(successor.ToAnArray());
        }

        public ChainingModification After<TProcessorOriginal>(IEnumerable<IProcessor> successors) where TProcessorOriginal : IProcessor
        {
            return After(ProcessorMatcher.ByType<TProcessorOriginal>(), successors);
        }

        public ChainingModification After(Type original, IProcessor successor)
        {
            return After(original, successor.ToAnArray());
        }

        public ChainingModification After(Type original, IEnumerable<IProcessor> successors)
        {
            return After(ProcessorMatcher.ByType(original), successors);
        }

        public ChainingModification After(IProcessor original, IProcessor successor)
        {
            return After(original, successor.ToAnArray());
        }

        public ChainingModification After(IProcessor original, IEnumerable<IProcessor> successors)
        {
            return After(original.GetMatcher(), successors);
        }

        public ChainingModification After(IProcessorMatcher matcher, IProcessor successor)
        {
            return After(matcher, successor.ToAnArray());
        }

        public ChainingModification After(IProcessorMatcher matcher, IEnumerable<IProcessor> successors)
        {
            configurations.AddLast(new AfterProcessorModification(matcher, successors));
            return this;
        }

        public ChainingModification Before<TProcessorOriginal, TProcessorBefore>() where TProcessorBefore : IProcessor, new() where TProcessorOriginal : IProcessor
        {
            return Before<TProcessorOriginal>(new TProcessorBefore());
        }

        public ChainingModification Before<TProcessorOriginal>(IProcessor predecessor) where TProcessorOriginal : IProcessor
        {
            return Before<TProcessorOriginal>(predecessor.ToAnArray());
        }

        public ChainingModification Before<TProcessorOriginal>(IEnumerable<IProcessor> predecessors) where TProcessorOriginal : IProcessor
        {
            return Before(ProcessorMatcher.ByType<TProcessorOriginal>(), predecessors);
        }

        public ChainingModification Before(Type original, IProcessor predecessor)
        {
            return Before(original, predecessor.ToAnArray());
        }

        public ChainingModification Before(Type original, IEnumerable<IProcessor> predecessors)
        {
            return Before(ProcessorMatcher.ByType(original), predecessors);
        }

        public ChainingModification Before(IProcessor original, IProcessor predecessor)
        {
            return Before(original, predecessor.ToAnArray());
        }

        public ChainingModification Before(IProcessor original, IEnumerable<IProcessor> predecessors)
        {
            return Before(original.GetMatcher(), predecessors);
        }

        public ChainingModification Before(IProcessorMatcher matcher, IProcessor predecessor)
        {
            return Before(matcher, predecessor.ToAnArray());
        }

        public ChainingModification Before(IProcessorMatcher matcher, IEnumerable<IProcessor> predecessors)
        {
            configurations.AddLast(new BeforeProcessorModification(matcher, predecessors));
            return this;
        }
        public IModificationConfiguration GetConfiguration()
        {
            return new ModificationConfigurationFacade(configurations);
        }

        public ChainingModification Instead<TProcessorOriginal, TProcessorInstead>() where TProcessorInstead : IProcessor, new() where TProcessorOriginal : IProcessor
        {
            return Instead<TProcessorOriginal>(new TProcessorInstead());
        }

        public ChainingModification Instead<TProcessorOriginal>(IProcessor substitute) where TProcessorOriginal : IProcessor
        {
            return Instead<TProcessorOriginal>(substitute.ToAnArray());
        }

        public ChainingModification Instead<TProcessorOriginal>(IEnumerable<IProcessor> substitutes) where TProcessorOriginal : IProcessor
        {
            return Instead(ProcessorMatcher.ByType<TProcessorOriginal>(), substitutes);
        }

        public ChainingModification Instead(Type original, IProcessor substitute)
        {
            return Instead(original, substitute.ToAnArray());
        }

        public ChainingModification Instead(Type original, IEnumerable<IProcessor> substitutes)
        {
            return Instead(ProcessorMatcher.ByType(original), substitutes);
        }

        public ChainingModification Instead(IProcessor original, IProcessor substitute)
        {
            return Instead(original, substitute.ToAnArray());
        }

        public ChainingModification Instead(IProcessor original, IEnumerable<IProcessor> substitutes)
        {
            return Instead(original.GetMatcher(), substitutes);
        }

        public ChainingModification Instead(IProcessorMatcher matcher, IProcessor substitute)
        {
            return Instead(matcher, substitute.ToAnArray());
        }

        public ChainingModification Instead(IProcessorMatcher matcher, IEnumerable<IProcessor> substitutes)
        {
            configurations.AddLast(new SubstituteProcessorModification(matcher, substitutes));
            return this;
        }

        public ChainingModification Remove<TProcessorOriginal>() where TProcessorOriginal : IProcessor
        {
            return Remove(ProcessorMatcher.ByType<TProcessorOriginal>());
        }

        public ChainingModification Remove(Type original)
        {
            return Remove(ProcessorMatcher.ByType(original));
        }

        public ChainingModification Remove(IProcessor original)
        {
            return Remove(original.GetMatcher());
        }

        public ChainingModification Remove(IProcessorMatcher matcher)
        {
            configurations.AddLast(new RemoveProcessorModification(matcher));
            return this;
        }
    }
}
