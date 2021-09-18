using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace AutoPipe.Tests.Integrations
{
    public class BinarySearchIntegrationTests
    {
        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public async void Should_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = await DependencyResolver.DefaultElementFinder.FindElement(data, number).ConfigureAwait(false);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public async void Should_Not_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = await DependencyResolver.DefaultElementFinder.FindElement(data, number).ConfigureAwait(false);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        [Theory(Skip = "Not optimized for bag usage")]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public async Task Should_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await DependencyResolver.PipelineElementFinder.FindElement(data, number).ConfigureAwait(false);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public async Task Should_Not_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await DependencyResolver.PipelineElementFinder.FindElement(data, number).ConfigureAwait(false);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        public class DataContainer : Bag
        {
            public DataContainer(int[] array, int elementToBeFound)
            {
                this["ElementToBeFound"] = elementToBeFound;
                this["Array"] = array;
            }

            public int[] Array { get => this.GetOrThrow<int[]>("Array"); }
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

            public IPipeline Runner { get; }

            public BinarySearchFinder(IPipeline runner)
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

                await Runner.Run(container).ConfigureAwait(false);

                return container.FoundIndex;
            }
        }

        public class BinarySearchRunnerPipeline : IPipeline
        {
            public IPipeline Finder { get; }

            public BinarySearchRunnerPipeline(IPipeline finder)
            {
                Finder = finder;
            }

            public IEnumerable<IProcessor> GetProcessors()
            {
                yield return SortArray;
                yield return RunSearch;
            }

            private Processor SortArray =>
                Processor.From(SortArrayImplementation);

            private Task SortArrayImplementation(Bag container)
            {
                if (container.Contains<int[]>("Array", out var array))
                    Array.Sort(array);

                return PipelineTask.CompletedTask;
            }

            private Processor RunSearch =>
                Processor.From(RunSearchImplementation);

            private Task RunSearchImplementation(Bag bag)
            {
                var endSearchIndex = bag.Int("EndSearchIndex");

                while (bag.Int("StartSearchIndex") <= endSearchIndex && bag.Bool("ElementFound"))
                {
                    Finder.Run(bag);
                }

                return PipelineTask.CompletedTask;
            }
        }

        public class BinarySearchAlgorythmPipeline : IPipeline
        {
            public IEnumerable<IProcessor> GetProcessors()
            {
                yield return SetCurrentIndexProcessor;
                yield return ResizeToLeftPartProcessor;
                yield return ResizeToRightPartProcessor;
                yield return TrySetFoundIndexProcessor;
            }

            private Processor SetCurrentIndexProcessor =>
                Processor.From(SetCurrentIndexImplementation);

            private Task SetCurrentIndexImplementation(Bag container)
            {
                container["CurrentIndex"] = (container.Int("StartSearchIndex") + container.Int("EndSearchIndex")) / 2;
                return PipelineTask.CompletedTask;
            }

            private Processor ResizeToRightPartProcessor =>
                Processor.From(ResizeToRightPartImplementation);

            private Task ResizeToRightPartImplementation(Bag container)
            {
                if (container.Int("ElementToBeFound") > container.Int("CurrentElement"))
                    container["StartSearchIndex"] = container.Int("CurrentIndex") + 1;

                return PipelineTask.CompletedTask;
            }

            private Processor ResizeToLeftPartProcessor =>
                Processor.From(ResizeToLeftPartImplementation);

            private Task ResizeToLeftPartImplementation(Bag container)
            {
                if (container.Int("ElementToBeFound") < container.Int("CurrentElement"))
                    container["EndSearchIndex"] = container.Int("CurrentIndex") - 1;

                return PipelineTask.CompletedTask;
            }

            private Processor TrySetFoundIndexProcessor =>
                Processor.From(TrySetFoundIndexImplementation);

            private Task TrySetFoundIndexImplementation(Bag container)
            {
                if (container.Int("CurrentElement") == container.Int("ElementToBeFound"))
                    container["FoundIndex"] = container.Int("CurrentIndex");

                return PipelineTask.CompletedTask;
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