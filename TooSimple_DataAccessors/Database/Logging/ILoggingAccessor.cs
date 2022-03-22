namespace TooSimple_DataAccessors.Database.Logging
{
    public interface ILoggingAccessor
    {
        Task<bool> LogMessageAsync(string? errorCode, string message);

    }
}
