namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    [Run(ClaimAllParameters = true)]
    public class RunClassRequireRunMethodNotParameterRequiredProcessor
    {
        [Run(ClaimAllParameters = false)]
        public object CheckProperty([Required(Message = "The parameter \'Message\' should be passed.")] string message)
        {
            return new { valid = message != null };
        }
    }
}
