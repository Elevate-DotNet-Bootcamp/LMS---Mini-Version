namespace LMS___Mini_Version.Features.Common
{
    /// <summary>
    /// Generic result wrapper for orchestrator operations.
    /// Replaces the old MediatorResult class.
    /// </summary>
    public class CommandResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public static CommandResult Fail(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };

        public static CommandResult Succeed(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };
    }
}
