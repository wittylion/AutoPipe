using System.Collections.Generic;

namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class OrderTestAutoProcessor11 : AutoProcessor
    {
        protected override IEnumerable<string> GetPropertyUpdateIdentifiers()
        {
            yield return "A";
        }

        [Run]
        public string AFirst(string second) { return second + nameof(OrderTestAutoProcessor10); }

        [Run]
        public string ASecond() { return nameof(OrderTestAutoProcessor10); }

        [Run]
        public void APropertyUser(string first, string second, string third) { }
    }
}
