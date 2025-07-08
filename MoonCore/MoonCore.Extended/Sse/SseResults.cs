using MoonCore.Sse;

namespace MoonCore.Extended.Sse;

public static class SseResults
{
    public static SseResult<T> ServerSideEvents<T>(IAsyncEnumerable<SseItem<T>> provider)
    {
        return new SseResult<T>(provider);
    }
}