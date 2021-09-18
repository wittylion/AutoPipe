using System.Collections.Generic;

namespace AutoPipe.Tests.Integrations
{
    public class DataSets
    {
        public static IEnumerable<object[]> GetIntegerArrayCollection()
        {
            yield return new object[] { new int[] { 66, 55, 77, 60, 1, 100, 21, 70 } };
            yield return new object[] { new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 } };
            yield return new object[] { new int[] { 222 } };
        }

        public static IEnumerable<object[]> GetIntegerArrayCollectionAndNumberFromIt()
        {
            yield return new object[] { new int[] { 66, 55, 77, 60, 1, 100, 21, 70 }, 70 };
            yield return new object[] { new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 3 };
            yield return new object[] { new int[] { 222 }, 222 };
        }

        public static IEnumerable<object[]> GetIntegerArrayCollectionAndNumberThatDoesNotBelongToIt()
        {
            yield return new object[] { new int[] { 66, 55, 77, 60, 1, 100, 21, 70 }, 2 };
            yield return new object[] { new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 0 };
            yield return new object[] { new int[] { 222 }, 232 };
        }
    }
}