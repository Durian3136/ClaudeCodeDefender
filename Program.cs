using ClaudeCodeDefender.Models;
using ClaudeCodeDefender.Services;
using ClaudeCodeDefender.UI;
using Spectre.Console;

namespace ClaudeCodeDefender;

internal static class Program
{
    private static int Main()
    {
        var logger = new FileLogger();
        var settings = new SettingsStore();
        var killer = new ProcessKiller();
        IMenu menu = new ConsoleMenu();

        AnsiConsole.MarkupLine("[bold cyan]ClaudeCodeDefender[/] — мониторинг VPN");
        logger.Info("Запуск приложения.");

        while (true)
        {
            var saved = settings.Load();
            var options = menu.Configure(saved);
            if (options is null)
            {
                logger.Warn("Пользователь отменил выбор. Выход.");
                return 0;
            }

            // сохраняем выбор
            saved.LastProcessName = options.TargetProcessName;
            saved.LastVpnInterfaceId = options.VpnInterfaceId;
            settings.Save(saved);

            logger.Info(
                $"Наблюдение: процесс=\"{options.TargetProcessName}\", " +
                $"адаптер=\"{options.VpnInterfaceId}\".");

            var monitor = new VpnMonitor(options.VpnInterfaceId);

            if (!monitor.IsVpnAlive())
            {
                logger.Warn("VPN не активен на момент старта наблюдения. " +
                            "Возврат в меню.");
                AnsiConsole.MarkupLine("[yellow]VPN сейчас неактивен — вернусь в меню.[/]");
                continue;
            }

            // цикл наблюдения
            while (true)
            {
                if (!monitor.IsVpnAlive())
                {
                    logger.Warn("VPN отключён. Убиваю процесс(-ы).");

                    var killed = killer.KillAll(options.TargetProcessName);

                    if (killed > 0)
                        logger.Info($"Убито процессов \"{options.TargetProcessName}\": {killed}.");
                    else
                        logger.Warn($"Процесс \"{options.TargetProcessName}\" не найден (уже закрыт?).");

                    AnsiConsole.MarkupLine("[red]VPN отвалился — процесс остановлен.[/]");
                    AnsiConsole.MarkupLine("[grey]Возврат в меню...[/]");
                    break;
                }

                Thread.Sleep(options.PollIntervalMs);
            }
        }
    }
}