namespace BlogManager.Core.Logger;

public interface IBlogManagerLogger
{
    void LogInformation(string message); 
    void LogWarning(string     message);
    void LogError(string       message);
    void LogError(string?      message, params object?[] args);
}