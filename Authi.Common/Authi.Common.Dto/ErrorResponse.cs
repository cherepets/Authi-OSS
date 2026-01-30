namespace Authi.Common.Dto
{
    public class ErrorResponse<T>(string? error) : OptionalResponse<T>(error) where T : class
    {
    }
}
