using System;
using System.Diagnostics;

class WireGuardClient
{
    // Путь к конфигурационному файлу WireGuard
    private const string ConfigFilePath = @"D:\test.wg";  // Путь к вашему файлу конфигурации
    // Путь к исполняемому файлу wg
    private const string WgPath = @"C:\Program Files\WireGuard\wg.exe";  // Путь к wg.exe

    // Функция для настройки WireGuard вручную
    public static void StartWireGuard()
    {
        try
        {
            Console.WriteLine("Настройка интерфейса WireGuard вручную...");

            // 1. Установить конфигурацию интерфейса с помощью wg
            ExecuteCommand(WgPath, $"setconf wg0 \"{ConfigFilePath}\"");

            // 2. Включить интерфейс (если нужно)
            EnableInterface("wg0");

            Console.WriteLine("WireGuard успешно настроен и подключен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при настройке WireGuard: {ex.Message}");
        }
    }

    // Функция для остановки WireGuard вручную
    public static void StopWireGuard()
    {
        try
        {
            Console.WriteLine("Остановка WireGuard...");

            // Отключить интерфейс (если нужно)
            DisableInterface("wg0");

            Console.WriteLine("WireGuard остановлен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при остановке WireGuard: {ex.Message}");
        }
    }

    // Функция для выполнения команды в командной строке
    private static void ExecuteCommand(string command, string arguments)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Команда завершилась с ошибкой: {error}");
            }
        }
    }

    // Функция для включения интерфейса
    private static void EnableInterface(string interfaceName)
    {
        ExecuteCommand("netsh", $"interface set interface name=\"{interfaceName}\" enabled");
    }

    // Функция для отключения интерфейса
    private static void DisableInterface(string interfaceName)
    {
        ExecuteCommand("netsh", $"interface set interface name=\"{interfaceName}\" disabled");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("WireGuard Client");
        Console.WriteLine("1. Запустить VPN");
        Console.WriteLine("2. Остановить VPN");
        Console.Write("Выберите действие: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                StartWireGuard();
                break;
            case "2":
                StopWireGuard();
                break;
            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }
}
