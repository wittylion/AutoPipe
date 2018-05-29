using System.Collections.Generic;

namespace Pipelines.Tests.Integrations
{
    public class DataSets
    {
        public static IEnumerable<object[]> GetIntegerArrayCollection()
        {
            yield return new object[] { new int[] { 66, 55, 77, 60, 1, 100, 21, 70 } };
            yield return new object[] { new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 } };
            yield return new object[] { new int[] { 222 } };
        }
    }
}