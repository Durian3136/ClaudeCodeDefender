using Spectre.Console;

namespace ClaudeCodeDefender.Services;

public sealed class FileLogger : ILogger
{
    private readonly string _path;
    private readonly object _lock = new();

    public FileLogger()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClaudeCodeDefender");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "defender.log");
    }

    public void Info(string message)  => Write("INFO",  message, "grey");
    public void Warn(string message)  => Write("WARN",  message, "yellow");
    public void Error(string message, Exception? ex = null)
    {
        Write("ERROR", message, "red");
        if (ex is not null) Write("ERROR", ex.ToString(), "red");
    }

    private void Write(string level, string message, string color)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        lock (_lock)
        {
            try { File.AppendAllText(_path, line + Environment.NewLine); }
            catch {}
        }
        AnsiConsole.MarkupLine($"[{color}]{Markup.Escape(line)}[/]");
    }
}