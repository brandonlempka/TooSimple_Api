namespace TooSimple_DataAccessors.Database.Logging
{
    public interface ILoggingAccessor
    {
        Task<bool> LogMessageAsync(string message, string? errorCode);

    }
}
