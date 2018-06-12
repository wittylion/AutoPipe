﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.ExtensionMethods;
using Pipelines.Implementations;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class BubbleSortIntegrationTest
    {

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollection), MemberType = typeof(DataSets))]
        public async Task Should_Sort_The_Data_By_Means_Of_Single_Processor(int[] data)
        {
            var bubbleSortProcessor = ActionProcessor.From<DataContainer>(container =>
                SwapElements(container.Array, container.CurrentPointer, container.CurrentPointer + 1))
                .If(container => container.CurrentElement > container.NextElement)
                .Then(container => container.GoNextElement())
                .While(container => container.HasNextElement())
                .Then(container => container.OneMoreTraverse())
                .While(container => container.CanTraverseOneMoreTime());

            await bubbleSortProcessor.Execute(new DataContainer() {Array = data});

            data.Should()
                .BeInAscendingOrder("because we are sorting in the descending order, using bubble sort algorythm.");
        }


        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollection), MemberType = typeof(DataSets))]
        public async Task Should_Sort_The_Data_By_Means_Of_Several_Processors(int[] data)
        {
            var swapElementsIfNeeded = ActionProcessor.From<DataContainer>(container =>
                    SwapElements(container.Array, container.CurrentPointer, container.CurrentPointer + 1))
                .If(container => container.CurrentElement > container.NextElement);

            var movePointerToNextElement = ActionProcessor.From<DataContainer>(container =>
                container.GoNextElement());

            var requestOneMoreTraverse = ActionProcessor.From<DataContainer>(container =>
                container.OneMoreTraverse());
            
            var bubbleSortProcessor = 
                swapElementsIfNeeded.Then(movePointerToNextElement)
                .While(container => container.HasNextElement())
                .Then(requestOneMoreTraverse)
                .While(container => container.CanTraverseOneMoreTime());

            await bubbleSortProcessor.Execute(new DataContainer() { Array = data });

            data.Should()
                .BeInAscendingOrder("because we are sorting in the descending order, using bubble sort algorythm.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollection), MemberType = typeof(DataSets))]
        public async Task Should_Sort_The_Data_By_Means_Of_Several_Pipelines(int[] data)
        {
            var args = new DataContainer() {Array = data};
            
            await GetSortingProcessors(args).RunProcessorsWhile(args, container => container.CanTraverseOneMoreTime());

            data.Should()
                .BeInAscendingOrder("because we are sorting in the descending order, using bubble sort algorythm.");
        }

        protected IEnumerable<IProcessor> GetSortingProcessors(DataContainer stateContainer)
        {
            yield return GetTraverseProcessors().RepeatProcessorsAsPipelineWhile(stateContainer.HasNextElement).ToProcessor();
            yield return ActionProcessor.From<DataContainer>(container =>
                container.OneMoreTraverse());
        }

        protected IEnumerable<SafeTypeProcessor<DataContainer>> GetTraverseProcessors()
        {
            yield return ActionProcessor.From<DataContainer>(container =>
                    SwapElements(container.Array, container.CurrentPointer, container.CurrentPointer + 1))
                .If(container => container.CurrentElement > container.NextElement);

            yield return ActionProcessor.From<DataContainer>(container =>
                container.GoNextElement());
        }

        protected void SwapElements(int[] array, int first, int second)
        {
            if (first > array.Length || first < 0)
                return;

            if (second > array.Length || second < 0)
                return;

            var temp = array[first];
            array[first] = array[second];
            array[second] = temp;
        }

        public class DataContainer
        {
            public int[] Array { get; set; }
            public int TraverseCount { get; set; }
            public int CurrentElement => Array[CurrentPointer];
            public int NextElement => Array[CurrentPointer + 1];
            public int CurrentPointer { get; set; }

            public void GoNextElement()
            {
                ++CurrentPointer;
            }

            public bool HasNextElement()
            {
                return CurrentPointer < Array.Length - 1;
            }

            public void ResetCurrentPointer()
            {
                CurrentPointer = 0;
            }

            public void OneMoreTraverse()
            {
                ResetCurrentPointer();
                ++TraverseCount;
            }

            public bool CanTraverseOneMoreTime()
            {
                return TraverseCount < Array.Length - 1;
            }
        }
    }
}