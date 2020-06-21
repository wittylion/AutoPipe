using Pipelines.ExtensionMethods;
using Pipelines.Implementations.Processors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    public class ChainingModification
    {
        protected LinkedList<IModificationConfiguration> configurations = new LinkedList<IModificationConfiguration>();

        public ChainingModification AfterAll<TProcessorAfter>() where TProcessorAfter : IProcessor, new()
        {
            AfterAll(new TProcessorAfter());
            return this;
        }

        public ChainingModification AfterAll(IEnumerable<IProcessor> successors)
        {
            configurations.AddLast(new AfterProcessorModification(ProcessorMatcher.True, successors));
            return this;
        }

        public ChainingModification AfterAll(params IProcessor[] successors)
        {
            AfterAll((IEnumerable<IProcessor>) successors);
            return this;
        }

        public ChainingModification After<TProcessorOriginal, TProcessorAfter>() where TProcessorAfter : IProcessor, new() where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>(new TProcessorAfter());
        }

        public ChainingModification After<TProcessorOriginal, TProcessorAfter1, TProcessorAfter2>()
            where TProcessorAfter1 : IProcessor, new()
            where TProcessorAfter2 : IProcessor, new()
            where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>(new TProcessorAfter1().ThenProcessor(new TProcessorAfter2()));
        }

        public ChainingModification After<TProcessorOriginal, TProcessorAfter1, TProcessorAfter2, TProcessorAfter3>()
            where TProcessorAfter1 : IProcessor, new()
            where TProcessorAfter2 : IProcessor, new()
            where TProcessorAfter3 : IProcessor, new()
            where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>(
                new TProcessorAfter1()
                    .ThenProcessor(new TProcessorAfter2())
                    .ThenProcessor(new TProcessorAfter3())
                );
        }

        public ChainingModification After<TProcessorOriginal>(params IProcessor[] successors) where TProcessorOriginal : IProcessor
        {
            return After<TProcessorOriginal>((IEnumerable<IProcessor>) successors);
        }

        public ChainingModification After<TProcessorOriginal>(IEnumerable<IProcessor> successors) where TProcessorOriginal : IProcessor
        {
            return After(ProcessorMatcher.ByType<TProcessorOriginal>(), successors);
        }

        public ChainingModification After(Type original, params IProcessor[] successors)
        {
            After(original, (IEnumerable<IProcessor>)successors);
            return this;
        }

        public ChainingModification After(Type original, IEnumerable<IProcessor> successors)
        {
            return After(ProcessorMatcher.ByType(original), successors);
        }

        public ChainingModification After(IProcessor original, params IProcessor[] successors)
        {
            After(original, (IEnumerable<IProcessor>)successors);
            return this;
        }

        public ChainingModification After(IProcessor original, IEnumerable<IProcessor> successors)
        {
            return After(original.GetMatcher(), successors);
        }

        public ChainingModification After(IProcessorMatcher matcher, params IProcessor[] successors)
        {
            After(matcher, (IEnumerable<IProcessor>) successors);
            return this;
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

        public ChainingModification Before<TProcessorOriginal>(params IProcessor[] predecessors) where TProcessorOriginal : IProcessor
        {
            return Before<TProcessorOriginal>((IEnumerable<IProcessor>) predecessors);
        }

        public ChainingModification Before<TProcessorOriginal>(IEnumerable<IProcessor> predecessors) where TProcessorOriginal : IProcessor
        {
            return Before(ProcessorMatcher.ByType<TProcessorOriginal>(), predecessors);
        }

        public ChainingModification Before(Type original, params IProcessor[] predecessors)
        {
            return Before(original, (IEnumerable<IProcessor>) predecessors);
        }

        public ChainingModification Before(Type original, IEnumerable<IProcessor> predecessors)
        {
            return Before(ProcessorMatcher.ByType(original), predecessors);
        }

        public ChainingModification Before(IProcessor original, params IProcessor[] predecessors)
        {
            return Before(original, (IEnumerable<IProcessor>)predecessors);
        }

        public ChainingModification Before(IProcessor original, IEnumerable<IProcessor> predecessors)
        {
            return Before(original.GetMatcher(), predecessors);
        }

        public ChainingModification Before(IProcessorMatcher matcher, params IProcessor[] predecessors)
        {
            return Before(matcher, (IEnumerable<IProcessor>) predecessors);
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

        public ChainingModification Insert<TProcessorNew>(int position) where TProcessorNew : IProcessor, new()
        {
            return Insert(position, new TProcessorNew());
        }

        public ChainingModification Insert(int position, params IProcessor[] insertions)
        {
            return Insert(position, (IEnumerable<IProcessor>) insertions);
        }

        public ChainingModification Insert(int position, IEnumerable<IProcessor> insertions)
        {
            this.configurations.AddLast(new InsertProcessorModification(position, insertions));
            return this;
        }

        public ChainingModification Instead<TProcessorOriginal, TProcessorInstead>() where TProcessorInstead : IProcessor, new() where TProcessorOriginal : IProcessor
        {
            return Instead<TProcessorOriginal>(new TProcessorInstead());
        }

        public ChainingModification Instead<TProcessorOriginal>(params IProcessor[] substitutes) where TProcessorOriginal : IProcessor
        {
            return Instead<TProcessorOriginal>((IEnumerable<IProcessor>) substitutes);
        }

        public ChainingModification Instead<TProcessorOriginal>(IEnumerable<IProcessor> substitutes) where TProcessorOriginal : IProcessor
        {
            return Instead(ProcessorMatcher.ByType<TProcessorOriginal>(), substitutes);
        }

        public ChainingModification Instead(Type original, params IProcessor[] substitutes)
        {
            return Instead(original, (IEnumerable<IProcessor>) substitutes);
        }

        public ChainingModification Instead(Type original, IEnumerable<IProcessor> substitutes)
        {
            return Instead(ProcessorMatcher.ByType(original), substitutes);
        }

        public ChainingModification Instead(IProcessor original, params IProcessor[] substitutes)
        {
            return Instead(original, (IEnumerable<IProcessor>) substitutes);
        }

        public ChainingModification Instead(IProcessor original, IEnumerable<IProcessor> substitutes)
        {
            return Instead(original.GetMatcher(), substitutes);
        }

        public ChainingModification Instead(IProcessorMatcher matcher, params IProcessor[] substitutes)
        {
            return Instead(matcher, (IEnumerable<IProcessor>) substitutes);
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

        public ChainingModification Remove<TProcessorOriginal1, TProcessorOriginal2>() 
            where TProcessorOriginal1 : IProcessor
            where TProcessorOriginal2 : IProcessor
        {
            return Remove(ProcessorMatcher.ByType<TProcessorOriginal1>(), 
                ProcessorMatcher.ByType<TProcessorOriginal2>());
        }

        public ChainingModification Remove<TProcessorOriginal1, TProcessorOriginal2, TProcessorOriginal3>()
            where TProcessorOriginal1 : IProcessor
            where TProcessorOriginal2 : IProcessor
            where TProcessorOriginal3 : IProcessor
        {
            return Remove(ProcessorMatcher.ByType<TProcessorOriginal1>(),
                ProcessorMatcher.ByType<TProcessorOriginal2>(),
                ProcessorMatcher.ByType<TProcessorOriginal3>());
        }

        public ChainingModification Remove(params Type[] types)
        {
            return Remove(types.Select(type => ProcessorMatcher.ByType(type)));
        }

        public ChainingModification Remove(params IProcessor[] refereces)
        {
            return Remove(refereces.Select(reference => reference.GetMatcher()));
        }

        public ChainingModification Remove(params IProcessorMatcher[] matchers)
        {
            return Remove((IEnumerable<IProcessorMatcher>)matchers);
        }

        public ChainingModification Remove(IEnumerable<IProcessorMatcher> matchers)
        {
            configurations.AddLast(new RemoveProcessorModification(matchers));
            return this;
        }
    }
}
