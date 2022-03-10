using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.SingleMethodMarkedTest
{
    public class CustomProcessor
    {
        public static readonly Guid guid = Guid.NewGuid();
        [Run]
        public object GetResult()
        {
            return guid.ToString();
        }
    }
}
