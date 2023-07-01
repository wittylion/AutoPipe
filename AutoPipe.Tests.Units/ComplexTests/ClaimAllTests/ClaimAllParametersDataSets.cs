using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests
{
    public class ClaimAllParametersDataSets
    {
        public static IEnumerable<object[]> GetBlockingExecutionProcessors()
        {
            return Assembly.GetExecutingAssembly().Types().ThatAreInNamespace("AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations")
                .Select(x => new object[] { x.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) });
        }
    }
}
