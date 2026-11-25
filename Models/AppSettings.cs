namespace ClaudeCodeDefender.Models;


// Модель для того что запоминать последний процесс и последний адаптер
public sealed class AppSettings
{
    public string? LastProcessName { get; set; }
    public string? LastVpnInterfaceId { get; set; }
}