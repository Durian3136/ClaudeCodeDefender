using System.Diagnostics;

namespace ClaudeCodeDefender.Services;

public sealed class ProcessKiller : IProcessKiller
{
    public int KillAll(string processName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(processName);

        var processes = Process.GetProcessesByName(processName);
        var killed = 0;

        foreach (var p in processes)
        {
            try
            {
                p.Kill();
                p.WaitForExit(2000);
                killed++;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Не удалось убить {processName} (PID {p.Id}): {ex.Message}");
            }
            finally
            {
                p.Dispose();
            }
        }

        return killed;
    }
}