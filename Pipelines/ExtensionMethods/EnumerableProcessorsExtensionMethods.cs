using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipelines.Implementations;

namespace Pipelines.ExtensionMethods
{
    public static class EnumerableProcessorsExtensionMethods
    {
        public static IPipeline ToPipeline(this IEnumerable<IProcessor> enumerable)
        {
            if (enumerable.IsNull())
            {
                return PredefinedPipeline.Empty;
            }

            return PredefinedPipeline.FromProcessors(enumerable);
        }

        public static SafeTypePipeline<T> ToPipeline<T>(this IEnumerable<SafeTypeProcessor<T>> enumerable)
        {
            if (enumerable.IsNull())
            {
                return PredefinedPipeline.GetEmpty<T>();
            }

            return PredefinedPipeline.FromProcessors(enumerable);
        }

        public static IEnumerable<IProcessor> RepeatProcessorsWhile(
            this IEnumerable<IProcessor> enumerable, Func<bool> condition)
        {
            if (condition.HasNoValue() || enumerable.HasNoValue())
            {
                yield break;
            }

            while (condition())
            {
                foreach (var processor in enumerable)
                {
                    yield return processor;
                }
            }
        }


        public static IEnumerable<SafeTypeProcessor<T>> RepeatProcessorsWhile<T>(
            this IEnumerable<SafeTypeProcessor<T>> enumerable, Func<bool> condition)
        {
            if (condition.HasNoValue() || enumerable.HasNoValue())
            {
                yield break;
            }

            while (condition())
            {
                foreach (var processor in enumerable)
                {
                    yield return processor;
                }
            }
        }

        public static IPipeline RepeatProcessorsAsPipelineWhile(
            this IEnumerable<IProcessor> enumerable, Func<bool> condition)
        {
            if (enumerable.HasNoValue())
            {
                return PredefinedPipeline.Empty;
            }

            return new RepeatingProcessorsWhileConditionPipeline(enumerable, condition);
        }

        public static SafeTypePipeline<T> RepeatProcessorsAsPipelineWhile<T>(
            this IEnumerable<SafeTypeProcessor<T>> enumerable, Func<bool> condition)
        {
            if (enumerable.IsNull())
            {
                return PredefinedPipeline.GetEmpty<T>();
            }

            return new RepeatingProcessorsWhileConditionPipeline<T>(enumerable, condition);
        }

        public static Task Run<TArgs>(this IEnumerable<IProcessor> enumerable, TArgs args)
        {
            return enumerable.Run(args, PipelineRunner.StaticInstance);
        }

        public static Task Run<TArgs>(this IEnumerable<IProcessor> enumerable, TArgs args, PipelineRunner runner)
        {
            runner = runner.Ensure(PipelineRunner.StaticInstance);
            return runner.RunProcessors(enumerable, args);
        }

        public static Task RunProcessorsWhile<TArgs>(this IEnumerable<IProcessor> enumerable, TArgs args,
            Predicate<TArgs> condition)
        {
            return enumerable.RunProcessorsWhile(args, condition, PipelineRunner.StaticInstance);
        }

        public static async Task RunProcessorsWhile<TArgs>(this IEnumerable<IProcessor> enumerable, TArgs args,
            Predicate<TArgs> condition, PipelineRunner runner)
        {
            runner = runner.Ensure(PipelineRunner.StaticInstance);
            while (condition(args))
            {
                await enumerable.Run(args, runner);
            }
        }

        public static IEnumerable<IProcessor> ThenProcessor(this IEnumerable<IProcessor> enumerable, IProcessor nextProcessor)
        {
            return enumerable.ThenProcessor(new[] {nextProcessor});
        }

        public static IEnumerable<IProcessor> ThenProcessor(this IEnumerable<IProcessor> enumerable, IEnumerable<IProcessor> nextProcessors)
        {
            if (enumerable.HasNoValue())
            {
                if (nextProcessors.HasNoValue())
                {
                    return Enumerable.Empty<IProcessor>();
                }

                return nextProcessors;
            }

            return enumerable.Concat(nextProcessors);
        }
    }
}