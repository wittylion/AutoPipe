namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class OrderTestAutoProcessor8 : AutoProcessor
    {
        [Run]
        public string GetFirst() { return nameof(OrderTestAutoProcessor8); }

        [Run]
        public string GetSecond() { return nameof(OrderTestAutoProcessor8); }

        [Run]
        public void APropertyUser(string first, string second, string third) { }
    }
}
