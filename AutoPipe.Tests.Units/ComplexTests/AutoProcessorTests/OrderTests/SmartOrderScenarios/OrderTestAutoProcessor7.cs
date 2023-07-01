namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class OrderTestAutoProcessor7 : AutoProcessor
    {
        [Run]
        public string GetFirst() { return nameof(OrderTestAutoProcessor7); }

        [Run]
        public string GetSecond() { return nameof(OrderTestAutoProcessor7); }

        [Run]
        public void APropertyUser(string first, string second) { }
    }
}
