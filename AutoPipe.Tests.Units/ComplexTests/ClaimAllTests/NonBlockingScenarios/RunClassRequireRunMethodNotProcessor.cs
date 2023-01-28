namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.NonBlockingScenarios
{
    [Run(ClaimAllParameters = true)]
    public class RunClassRequireRunMethodNotProcessor
    {
        [Run(ClaimAllParameters = false)]
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
