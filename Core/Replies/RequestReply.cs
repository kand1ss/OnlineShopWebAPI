using System.Diagnostics.CodeAnalysis;

namespace CatalogManagementService.Application.Replies;

public class RequestReply<T>
{
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; init; }
    public string? Error { get; init; }
    public T? Result { get; init; }
    
    public static RequestReply<T> Ok(T result) => new() { Success = true, Result = result };
    public static RequestReply<T> Fail(string error) => new() { Success = false, Error = error };
}