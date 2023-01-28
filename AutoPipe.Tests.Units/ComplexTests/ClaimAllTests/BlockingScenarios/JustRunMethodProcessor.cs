namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    public class JustRunMethodProcessor
    {
        [Run(ClaimAllParameters = true)]
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
