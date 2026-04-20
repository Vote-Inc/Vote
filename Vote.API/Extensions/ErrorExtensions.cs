namespace Vote.API.Extensions;

public static class ErrorExtensions
{
    public static object ToResponse(this Error error) => new
    {
        code = error.Code,
        message = error.Message
    };
}