using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserChecker.Server.Model;
using System.IO;
using File = System.IO.File;

namespace UserChecker.ClientTgBot;

class Program
{
    static async Task Main()
    {
        using var cts = new CancellationTokenSource();

        BotDataBaseManager bot = new BotDataBaseManager(File.ReadLines("TOKEN.txt").First(), cts, new DataRepository(),
            new List<Operation>{ 
                new Operation("ПУСТОЙ ТАСК", RoleDB.User, EmptyOnPressButton, EmptyOnPressButton), 
                new Operation("ПУСТОЙ ТАСК 2", RoleDB.Admin, EmptyOnPressButton, EmptyOnPressButton), 
                new Operation("ПУСТОЙ ТАСК 3", RoleDB.Admin, EmptyOnPressButton, EmptyOnPressButton), 
                new Operation("ПУСТОЙ ТАСК 4", RoleDB.Admin, EmptyOnPressButton, EmptyOnPressButton)});
        
        Console.WriteLine($"bot is running... Press Enter to terminate");
        Console.ReadLine();
        await cts.CancelAsync();
    }

    static Task EmptyOnPressButton(Message msg)
    {
        return Task.CompletedTask;
    }
}