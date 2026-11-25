namespace ClaudeCodeDefender.Models;

public sealed class DefenderOptions
{
    public required string TargetProcessName { get; init; }
    public required string VpnInterfaceId { get; init; }
    public int PollIntervalMs { get; init; } = 1000;
}