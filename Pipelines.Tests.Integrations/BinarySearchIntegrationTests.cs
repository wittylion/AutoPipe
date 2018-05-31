using System;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.ExtensionMethods;
using Pipelines.Implementations;
using Xunit;

namespace Pipelines.Tests.Integrations
{
    public class BinarySearchIntegrationTests
    {
        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public void Should_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = BinarySearch(data, number);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public void Should_Not_Find_The_Number_In_Data_Array_By_A_Traditional_Implementation(int[] data, int number)
        {
            var mid = BinarySearch(data, number);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberFromIt), MemberType = typeof(DataSets))]
        public async Task Should_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await BinarySearchWithProcessors(data, number);

            data[mid].Should()
                .Be(number, "because we are finding this element in the collection.");
        }

        [Theory]
        [MemberData(nameof(DataSets.GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt), MemberType = typeof(DataSets))]
        public async Task Should_Not_Find_The_Number_In_Data_Array_By_Means_Of_Processors(int[] data, int number)
        {
            var mid = await BinarySearchWithProcessors(data, number);

            mid.Should()
                .Be(-1, "because we element doesn't exist in collection.");
        }

        protected int BinarySearch(int[] data, int number, int? min = null, int? max = null)
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

            return result ?? -1;
        }

        protected async Task<int> BinarySearchWithProcessors(int[] data, int number, int? min = null, int? max = null)
        {
            Array.Sort(data);

            var container = new DataContainer(data, number)
            {
                Min = min ?? 0,
                Max = max ?? data.Length - 1
            };

            await ActionProcessor.From<DataContainer>(dataContainer =>
                {
                    int mid = (dataContainer.Min + dataContainer.Max) / 2;
                    var value = dataContainer.Array[mid];
                    if (dataContainer.ElementToBeFound == value)
                    {
                        dataContainer.FoundIndex = mid;
                    }

                    if (dataContainer.ElementToBeFound < value)
                    {
                        dataContainer.Max = mid - 1;
                    }

                    if (dataContainer.ElementToBeFound > value)
                    {
                        dataContainer.Min = mid + 1;
                    }
                })
                .While(dataContainer => dataContainer.Min <= dataContainer.Max && !dataContainer.FoundIndex.HasValue)
                .Execute(container);

            return container.FoundIndex ?? -1;
        }

        public class DataContainer
        {
            public DataContainer(int[] array, int elementToBeFound)
            {
                ElementToBeFound = elementToBeFound;
                Array = array;
            }

            public int[] Array { get; }
            public int? FoundIndex { get; set; }
            public int? FoundElement => FoundIndex.HasValue ? (int?) Array[FoundIndex.Value] : null;

            public int Min { get; set; }
            public int Max { get; set; }

            public int ElementToBeFound { get; }
        }
    }
}