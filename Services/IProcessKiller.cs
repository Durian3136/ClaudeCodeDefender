namespace ClaudeCodeDefender.Services;

public interface IProcessKiller
{
    int KillAll(string processName);
}