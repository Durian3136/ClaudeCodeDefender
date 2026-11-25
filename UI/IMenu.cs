using ClaudeCodeDefender.Models;

namespace ClaudeCodeDefender.UI;

public interface IMenu
{
    DefenderOptions? Configure(AppSettings settings);
}