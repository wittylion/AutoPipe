using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        [Theory()]
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

            public int StartSearchIndex { get; set; }
            public int EndSearchIndex { get; set; }

            public int ElementToBeFound { get; }
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
                Expression<Func<int, bool>> e = (int foundIndex) => foundIndex != -1;
                Expression<Func<int, int[], int>> b = (int currentIndex, int[] array) => array[currentIndex];
                Expression<Func<int, int, bool>> c = (int startSearchIndex, int endSearchIndex) => startSearchIndex <= endSearchIndex;
                var bag = new Bag(container)
                    .Computed("ElementFound", e)
                    .Computed("CurrentElement", b)
                    .Computed("NotEnded", c);

                await Runner.Run(bag).ConfigureAwait(false);

                return bag.Int("FoundIndex");
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
                yield return EnsureStartAndEnd;
                yield return RunSearch;
            }

            private IProcessor SortArray =>
                ActionProcessor.From(SortArrayImplementation);

            private Task SortArrayImplementation(Bag container)
            {
                if (container.Contains<int[]>("Array", out var array))
                    Array.Sort(array);

                return PipelineTask.CompletedTask;
            }

            private IProcessor EnsureStartAndEnd =>
                ActionProcessor.From(EnsureStartAndEndImplementation);

            private Task EnsureStartAndEndImplementation(Bag container)
            {
                if (container.DoesNotContain<object>("EndSearchIndex"))
                {
                    container["EndSearchIndex"] = container.Get<int[]>("Array").Length - 1;
                }

                return PipelineTask.CompletedTask;
            }

            private IProcessor RunSearch =>
                ActionProcessor.From(RunSearchImplementation);

            private async Task RunSearchImplementation(Bag bag)
            {
                while (bag.Bool("NotEnded") && !bag.Bool("ElementFound"))
                {
                    await Finder.Run(bag);
                }
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

            private IProcessor SetCurrentIndexProcessor =>
                ActionProcessor.From(SetCurrentIndexImplementation);

            private Task SetCurrentIndexImplementation(Bag container)
            {
                container["CurrentIndex"] = (container.Int("StartSearchIndex") + container.Int("EndSearchIndex")) / 2;
                return PipelineTask.CompletedTask;
            }

            private IProcessor ResizeToRightPartProcessor =>
                ActionProcessor.From(ResizeToRightPartImplementation);

            private Task ResizeToRightPartImplementation(Bag bag)
            {
                if (bag.Int("ElementToBeFound") > bag.Int("CurrentElement"))
                    bag["StartSearchIndex"] = bag.Int("CurrentIndex") + 1;

                return PipelineTask.CompletedTask;
            }

            private IProcessor ResizeToLeftPartProcessor =>
                ActionProcessor.From(ResizeToLeftPartImplementation);

            private Task ResizeToLeftPartImplementation(Bag bag)
            {
                if (bag.Int("ElementToBeFound") < bag.Int("CurrentElement"))
                    bag["EndSearchIndex"] = bag.Int("CurrentIndex") - 1;

                return PipelineTask.CompletedTask;
            }

            private IProcessor TrySetFoundIndexProcessor =>
                ActionProcessor.From(TrySetFoundIndexImplementation);

            private Task TrySetFoundIndexImplementation(Bag bag)
            {
                if (bag.Int("CurrentElement") == bag.Int("ElementToBeFound"))
                    bag["FoundIndex"] = bag.Int("CurrentIndex");

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