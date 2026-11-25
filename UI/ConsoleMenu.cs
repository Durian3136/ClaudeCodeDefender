using ClaudeCodeDefender.Models;
using Spectre.Console;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ClaudeCodeDefender.UI;

public sealed class ConsoleMenu : IMenu
{
    private const string RefreshToken = "⟳  Обновить список";

    public DefenderOptions? Configure(AppSettings settings)
    {
        var processName = PromptProcess(settings.LastProcessName);
        if (processName is null) return null;

        var ifaceId = PromptInterface(settings.LastVpnInterfaceId);
        if (ifaceId is null) return null;

        return new DefenderOptions
        {
            TargetProcessName = processName,
            VpnInterfaceId = ifaceId
        };
    }

    // ---------- Процессы ----------
    private static string? PromptProcess(string? last)
    {
        while (true)
        {
            var names = Process.GetProcesses()
                .Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle))
                .Select(p => p.ProcessName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (names.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Нет процессов с окном.[/]");
                return null;
            }

            var choices = new List<string>(names) { RefreshToken };

            var prompt = new SelectionPrompt<string>()
                .Title("Выберите процесс для [red]убийства[/] при падении VPN:")
                .PageSize(15)
                .MoreChoicesText("[grey](стрелки вверх/вниз)[/]")
                .HighlightStyle(new Style(foreground: Color.Yellow))
                .AddChoices(choices);

            if (!string.IsNullOrWhiteSpace(last) && names.Contains(last, StringComparer.OrdinalIgnoreCase))
            {
                prompt.DefaultValue = last;
            }

            string choice = AnsiConsole.Prompt(prompt);

            if (choice == RefreshToken) continue;
            return choice;
        }
    }

    // ---------- Сетевые адаптеры ----------
    private static string? PromptInterface(string? lastId)
    {
        while (true)
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .OrderBy(ni => ni.Name, StringComparer.OrdinalIgnoreCase)
                .Select(ni => new AdapterChoice(
                    ni.Id,
                    Markup.Escape($"{ni.Name}  —  {ni.Description}  [{ni.OperationalStatus}]")))
                .ToList();

            if (adapters.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Сетевые адаптеры не найдены.[/]");
                return null;
            }

            var choices = new List<AdapterChoice>(adapters)
        {
            new AdapterChoice("__refresh__", RefreshToken)
        };

            var prompt = new SelectionPrompt<AdapterChoice>()
                .Title("Выберите [cyan]VPN-адаптер[/], за которым наблюдать:")
                .PageSize(15)
                .MoreChoicesText("[grey](стрелки вверх/вниз)[/]")
                .UseConverter(a => a.Display)
                .AddChoices(choices)
                .HighlightStyle(new Style(foreground: Color.Yellow));

            if (!string.IsNullOrWhiteSpace(lastId))
            {
                var match = adapters.FirstOrDefault(a => a.Id == lastId);
                if (match is not null) prompt.DefaultValue = match;
            }

            var choice = AnsiConsole.Prompt(prompt);

            if (choice.Id == "__refresh__") continue;
            return choice.Id;
        }
    }

    private sealed record AdapterChoice(string Id, string Display);
}