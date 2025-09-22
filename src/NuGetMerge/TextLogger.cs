using NuGet.Common;

namespace NuGetMerge;

public sealed class TextLogger : ILogger
{
    private readonly TextWriter _writer;

    private readonly LogLevel _logLevel;

    public TextLogger(TextWriter writer, LogLevel logLevel)
    {
        _writer = writer;
        _logLevel = logLevel;
    }

    public LogLevel LogLevel => _logLevel;

    public void Log(ILogMessage message) => Log(message.Level, message.Message);

    public Task LogAsync(ILogMessage message) => LogAsync(message.Level, message.Message);

    public void Log(LogLevel level, string? data)
    {
        if (level >= _logLevel)
            _writer.WriteLine(data);
    }

    public Task LogAsync(LogLevel level, string? data)
        => level >= _logLevel ? _writer.WriteLineAsync(data) : Task.CompletedTask;

    public void LogDebug(string? data) => Log(LogLevel.Debug, data);

    public void LogVerbose(string? data) => Log(LogLevel.Verbose, data);

    public void LogInformation(string? data) => Log(LogLevel.Information, data);

    public void LogInformationSummary(string? data) => Log(LogLevel.Information, data);

    public void LogMinimal(string? data) => Log(LogLevel.Minimal, data);

    public void LogWarning(string? data) => Log(LogLevel.Warning, data);

    public void LogError(string? data) => Log(LogLevel.Error, data);
}
