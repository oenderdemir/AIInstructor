namespace AIInstructor.src.Shared.Exceptions
{
    public class BaseException: Exception
    {
        public BaseException(string? message) : base(message)
        {
        }

        public int ErrorCode { get; set; }
    }
}
