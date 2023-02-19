using System.Collections.Generic;

namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class OrderTestAutoProcessor10 : AutoProcessor
    {
        protected override IEnumerable<string> GetPropertyEnsureIdentifiers()
        {
            yield return "Want";
        }

        [Run]
        public string WantFirst(string second) { return second + nameof(OrderTestAutoProcessor10); }

        [Run]
        public string WantSecond() { return nameof(OrderTestAutoProcessor10); }

        [Run]
        public void APropertyUser(string first, string second, string third) { }
    }
}
