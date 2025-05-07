namespace Core.Contracts;

public interface IRequestProcessor<in T, TResult>
{
    Task<TResult> Process(T data);
}