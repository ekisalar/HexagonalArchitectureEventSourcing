// Inside your Adapter.Logger project

using BlogManager.Core.Logger;
using Serilog;

namespace BlogManager.Adapter.Logger;

public class SerilogAdapter : IBlogManagerLogger
{
    private readonly ILogger _logger = new LoggerConfiguration()
                                      .WriteTo.Console()
                                      .CreateLogger();

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

   
    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogError(string message)
    {
        _logger.Error(message);
    }

    public void LogError(string? message, params object?[] args)
    {
        _logger.Error(message, args);
    }
}