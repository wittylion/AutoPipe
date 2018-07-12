using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.ExtensionMethods;
using Pipelines.Implementations.Processors;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class BinarySearchIntegrationTests
    {
        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public async void Should_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = await DependencyResolver.DefaultElementFinder.FindElement(data, number);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public async void Should_Not_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = await DependencyResolver.DefaultElementFinder.FindElement(data, number);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public async Task Should_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await DependencyResolver.PipelineElementFinder.FindElement(data, number);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public async Task Should_Not_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await DependencyResolver.PipelineElementFinder.FindElement(data, number);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        public class DataContainer
        {
            public DataContainer(int[] array, int elementToBeFound)
            {
                ElementToBeFound = elementToBeFound;
                Array = array;
            }

            public int[] Array { get; }
            public int FoundIndex { get; set; } = -1;
            public int CurrentIndex { get; set; }
            public int CurrentElement => Array[CurrentIndex];

            public int StartSearchIndex { get; set; }
            public int EndSearchIndex { get; set; }

            public int ElementToBeFound { get; }

            public bool ElementFound()
            {
                return FoundIndex != -1;
            }
        }

        public static class DependencyResolver
        {
            public static IElementFinder PipelineElementFinder = new BinarySearchFinder(new BinarySearchRunnerPipeline(new BinarySearchAlgorythmPipeline()));
            public static IElementFinder DefaultElementFinder = new DefaultBinarySearchImplementation();
        }

        public interface IElementFinder
        {
            Task<int> FindElement(int[] data, int number, int? min = null, int? max = null);
        }

        public class BinarySearchFinder : IElementFinder
        {

            public SafeTypePipeline<DataContainer> Runner { get; }

            public BinarySearchFinder(SafeTypePipeline<DataContainer> runner)
            {
                Runner = runner;
            }

            public async Task<int> FindElement(int[] data, int number, int? min = null, int? max = null)
            {
                var container = new DataContainer(data, number)
                {
                    StartSearchIndex = min ?? 0,
                    EndSearchIndex = max ?? data.Length - 1
                };

                await Runner.Run(container);

                return container.FoundIndex;
            }
        }

        public class BinarySearchRunnerPipeline : SafeTypePipeline<DataContainer>
        {
            public SafeTypePipeline<DataContainer> Finder { get; }

            public BinarySearchRunnerPipeline(SafeTypePipeline<DataContainer> finder)
            {
                Finder = finder;
            }

            public override IEnumerable<SafeTypeProcessor<DataContainer>> GetProcessorsOfType()
            {
                yield return SortArray;
                yield return RunSearch;
            }

            private SafeTypeProcessor<DataContainer> SortArray =>
                ActionProcessor.FromAction<DataContainer>(SortArrayImplementation);

            private Task SortArrayImplementation(DataContainer container)
            {
                if (container.Array != null)
                    Array.Sort(container.Array);

                return Task.CompletedTask;
            }

            private SafeTypeProcessor<DataContainer> RunSearch =>
                ActionProcessor.FromAction<DataContainer>(RunSearchImplementation);

            private Task RunSearchImplementation(DataContainer container)
            {
                Finder.RunPipelineWhile(container,
                        context => context.StartSearchIndex <= context.EndSearchIndex && !context.ElementFound());

                return Task.CompletedTask;
            }
        }

        public class BinarySearchAlgorythmPipeline : SafeTypePipeline<DataContainer>
        {
            public override IEnumerable<SafeTypeProcessor<DataContainer>> GetProcessorsOfType()
            {
                yield return SetCurrentIndexProcessor;
                yield return ResizeToLeftPartProcessor;
                yield return ResizeToRightPartProcessor;
                yield return TrySetFoundIndexProcessor;
            }

            private SafeTypeProcessor<DataContainer> SetCurrentIndexProcessor =>
                ActionProcessor.FromAction<DataContainer>(SetCurrentIndexImplementation);

            private Task SetCurrentIndexImplementation(DataContainer container)
            {
                container.CurrentIndex = (container.StartSearchIndex + container.EndSearchIndex) / 2;
                return Task.CompletedTask;
            }

            private SafeTypeProcessor<DataContainer> ResizeToRightPartProcessor =>
                ActionProcessor.FromAction<DataContainer>(ResizeToRightPartImplementation);

            private Task ResizeToRightPartImplementation(DataContainer container)
            {
                if (container.ElementToBeFound > container.CurrentElement)
                    container.StartSearchIndex = container.CurrentIndex + 1;

                return Task.CompletedTask;
            }

            private SafeTypeProcessor<DataContainer> ResizeToLeftPartProcessor =>
                ActionProcessor.FromAction<DataContainer>(ResizeToLeftPartImplementation);

            private Task ResizeToLeftPartImplementation(DataContainer container)
            {
                if (container.ElementToBeFound < container.CurrentElement)
                    container.EndSearchIndex = container.CurrentIndex - 1;

                return Task.CompletedTask;
            }

            private SafeTypeProcessor<DataContainer> TrySetFoundIndexProcessor =>
                ActionProcessor.FromAction<DataContainer>(TrySetFoundIndexImplementation);

            private Task TrySetFoundIndexImplementation(DataContainer container)
            {
                if (container.CurrentElement == container.ElementToBeFound)
                    container.FoundIndex = container.CurrentIndex;

                return Task.CompletedTask;
            }
        }
    }

    public class DefaultBinarySearchImplementation : BinarySearchIntegrationTests.IElementFinder
    {
        public Task<int> FindElement(int[] data, int number, int? min = null, int? max = null)
        {
            Array.Sort(data);
            var minVal = min ?? 0;
            var maxVal = max ?? data.Length - 1;
            int? result = null;

            while (minVal <= maxVal && !result.HasValue)
            {
                int mid = (minVal + maxVal) / 2;
                var value = data[mid];
                if (number == value)
                {
                    result = mid;
                }

                if (number < value)
                {
                    maxVal = mid - 1;
                }

                if (number > value)
                {
                    minVal = mid + 1;
                }
            }

            return Task.FromResult(result ?? -1);
        }
    }
}