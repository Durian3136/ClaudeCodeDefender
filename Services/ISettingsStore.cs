using ClaudeCodeDefender.Models;

namespace ClaudeCodeDefender.Services;

public interface ISettingsStore
{
    AppSettings Load();
    void Save(AppSettings settings);
}