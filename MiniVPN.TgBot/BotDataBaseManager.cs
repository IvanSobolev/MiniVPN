using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserChecker.Server.Model;

namespace UserChecker.ClientTgBot;

public class BotDataBaseManager
{
    private readonly TelegramBotClient _bot;
    private readonly CancellationTokenSource _cts;
    private readonly IDataRepository _dataRepository;
    private readonly List<Operation> _operations;
    private Dictionary<long, int> _userStates;
    

    public BotDataBaseManager(string token, CancellationTokenSource cts, IDataRepository dataRepository, List<Operation> operations)
    {
        _bot = new TelegramBotClient(token);
        _cts = cts;
        _dataRepository = dataRepository;
        _userStates = new();
        _operations = operations;
        
        _bot.OnError += OnError;
        _bot.OnMessage += OnMessage;
        _bot.OnUpdate += OnUpdate;
    }

    

    Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
        
    async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is null) return;
            
        UserDB? user = await _dataRepository.GetByTgId(msg.Chat.Id);
            
        if (user == null)
        {
            await _bot.SendMessage(msg.Chat, $"you are not user your id {msg.Chat.Id}");
            return;
        }
            
        if (msg.Text == "/start")
        {
            await _bot.SendMessage(msg.Chat, $"Id телеграма: `{user.TgId}`\n" +
                                                $"Имя пользователя: `{user.Name}`\n" +
                                                $"Роль: `{ user.roleDb.ToString()}`\n" +
                                                $"Актуальный код: `{user.ActualVpnCode}`\n" +
                                                $"Код активен `{(user.PaidUntil - DateTime.Now).Days}` суток", 
                parseMode: ParseMode.Markdown,
                replyMarkup: await GetInlineKeyboardForUser(user), 
                cancellationToken: _cts.Token);
        }

        if (_userStates.ContainsKey(msg.Chat.Id))
        {
            if (_operations[_userStates[msg.Chat.Id]].NeedRoleDb == user.roleDb)
            {
                await _operations[_userStates[msg.Chat.Id]].OnSendMassageAfterOperation(msg);
                _userStates.Remove(msg.Chat.Id);
            }
        }
    }
    
    Task<InlineKeyboardMarkup> GetInlineKeyboardForUser(UserDB user)
    {
        List<List<InlineKeyboardButton>> lines = new List<List<InlineKeyboardButton>>{new List<InlineKeyboardButton>()};
                
        for (int i = 0; i < _operations.Count; i++)
        {
            if (_operations[i].NeedRoleDb == user.roleDb)
            {
                lines.Last().Add(InlineKeyboardButton.WithCallbackData(_operations[i].Name, i.ToString()));
                if (lines.Last().Count() == 2)
                { lines.Add(new List<InlineKeyboardButton>()); }
            }
        }
        return Task.FromResult(new InlineKeyboardMarkup(lines));
    }
    
    async Task OnUpdate(Update update)
    {
        if (update is { CallbackQuery: { } query })
        {
            UserDB? user = await _dataRepository.GetByTgId(query.Message!.Chat.Id);
                
            if (user == null)
            {
                await _bot.SendMessage(query.Message!.Chat, $"you are not user your id {query.Message!.Chat.Id}");
                return;
            }

            int operationIndex = 0;
            try
            { operationIndex = Convert.ToInt32(query.Data); }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
                
            if (_operations[operationIndex].NeedRoleDb == user.roleDb)
            {
                await _operations[operationIndex].OnPressButton(query.Message!);
                _userStates[query.Message!.Chat.Id] = operationIndex;
            }
        }
            
            // if (userStates.ContainsKey(msg.Chat.Id))
            // {
            //     switch (userStates[msg.Chat.Id])
            //     {
            //         case "getId":
            //         {
            //             UserDB? user1 = await dataRepository.GetById(Convert.ToInt64(msg.Text));
            //         
            //             if (user == null)
            //             {
            //                 await bot.SendMessage(msg.Chat,"Not found");
            //             }
            //         
            //             await bot.SendMessage(msg.Chat, $"Id: `{user.Id}`\n" +
            //                                             $"Id телеграма: `{user.TgId}`\n" +
            //                                             $"Имя пользователя: `{user.Name}`\n" +
            //                                             $"Актуальный код: `{user.ActualVpnCode}`\n" +
            //                                             $"Роль: `{ user.roleDb.ToString()}`\n" +
            //                                             $"Код работает: `{(user.PaidUntil - DateTime.Now).Days}` сутки", 
            //                 parseMode: ParseMode.Markdown);
            //             userStates.Remove(chatId);
            //             break;
            //         }
            //         case "getTgId":
            //         {
            //             UserDB? user1 = await dataRepository.GetByTgId(Convert.ToInt64(msg.Text));
            //         
            //             if (user == null)
            //             {
            //                 await bot.SendMessage(msg.Chat,"Not found");
            //             }
            //         
            //             await bot.SendMessage(msg.Chat, $"Id: `{user.Id}`\n" +
            //                                             $"Id телеграма: `{user.TgId}`\n" +
            //                                             $"Имя пользователя: `{user.Name}`\n" +
            //                                             $"Актуальный код: `{user.ActualVpnCode}`\n" +
            //                                             $"Роль: `{ user.roleDb.ToString()}`\n" +
            //                                             $"Код работает: `{(user.PaidUntil - DateTime.Now).Days}` сутки", 
            //                 parseMode: ParseMode.Markdown);
            //             userStates.Remove(chatId);
            //             break;
            //         }
            //         case "add":
            //         {
            //             string text = msg.Text;
            //             try
            //             {
            //                 UserRequestDTO user1 = new UserRequestDTO(
            //                     Convert.ToInt64(text.Split("\n")[0]),
            //                     text.Split("\n")[1],
            //                     Convert.ToDateTime(text.Split("\n")[2]),
            //                     text.Split("\n")[3]);
            //                 await dataRepository.AddUser(user);
            //                 await bot.SendMessage(msg.Chat, "Пользователь добавлен");
            //             }
            //             catch
            //             {
            //                 await bot.SendMessage(msg.Chat, "Пользователь не добавлен");
            //             }
            //             
            //             userStates.Remove(chatId);
            //             break;
            //         }
            //         case "update":
            //         {
            //             string text = msg.Text;
            //             try
            //             {
            //                 UserDB user1 = new UserDB(
            //                     Convert.ToInt64(text.Split("\n")[0]),
            //                     Convert.ToInt64(text.Split("\n")[1]),
            //                     text.Split("\n")[2],
            //                     text.Split("\n")[3],
            //                     Convert.ToInt32(text.Split("\n")[4]),
            //                     Convert.ToDateTime(text.Split("\n")[5]));
            //                 await dataRepository.UpdateUserById(user);
            //                 await bot.SendMessage(msg.Chat, "Пользователь обновлен");
            //             }
            //             catch
            //             {
            //                 await bot.SendMessage(msg.Chat, "Пользователь не обновлен");
            //             }
            //             
            //             userStates.Remove(chatId);
            //             break;
            //         }
            //     }
            // }
        
        
            // switch (query.Data)
                // {
                //     case "GetAll":
                //     {
                //         IEnumerable<UserDB>? users = await dataRepository.GetAll();
                //     
                //         if (users == null)
                //         {
                //             await bot.SendMessage(query.Message!.Chat,"Data is null");
                //         }
                //     
                //         string text = "";
                //         foreach (var user1 in users)
                //         {
                //             await bot.SendMessage(query.Message!.Chat, $"Id: `{user.Id}`\n" +
                //                                                        $"Id телеграма: `{user.TgId}`\n" +
                //                                                        $"Имя пользователя: `{user.Name}`\n" +
                //                                                        $"Актуальный код: `{user.ActualVpnCode}`\n" +
                //                                                        $"Роль: `{ user.roleDb.ToString()}`\n" +
                //                                                        $"Код работает: `{(user.PaidUntil - DateTime.Now).Days}` сутки", 
                //                 parseMode: ParseMode.Markdown);
                //         }
                //         break;
                //     }
                //     case "getId":
                //     {
                //         await bot.SendMessage(query.Message!.Chat,"Отправь id пользователя:");
                //         userStates[query.Message!.Chat.Id] = "getId";
                //         break;
                //     }
                //     case "getTgId":
                //     {
                //         await bot.SendMessage(query.Message!.Chat,"Отправь телеграмм id пользователя:");
                //         userStates[query.Message!.Chat.Id] = "getTgId";
                //         break;
                //     }
                //     case "getSorted":
                //     {
                //         IEnumerable<UserDB>? users = await dataRepository.GetAllSorted();
                //     
                //         if (users == null)
                //         {
                //             await bot.SendMessage(query.Message!.Chat,"Data is null");
                //         }
                //     
                //         string text = "";
                //         foreach (var user1 in users)
                //         {
                //             await bot.SendMessage(query.Message!.Chat, $"Id: `{user.Id}`\n" +
                //                                                        $"Id телеграма: `{user.TgId}`\n" +
                //                                                        $"Имя пользователя: `{user.Name}`\n" +
                //                                                        $"Актуальный код: `{user.ActualVpnCode}`\n" +
                //                                                        $"Роль: `{ user.roleDb.ToString()}`\n" +
                //                                                        $"Код работает: `{(user.PaidUntil - DateTime.Now).Days}` сутки", 
                //                 parseMode: ParseMode.Markdown);
                //         }
                //         break;
                //     }
                //     case "add":
                //     {
                //         await bot.SendMessage(query.Message!.Chat,$"Отправь данные обновлаения в формате\ntgId\nname\npaidUntile({DateTime.Now.ToString()})\nactualCode");
                //         userStates[query.Message!.Chat.Id] = "add";
                //         break;
                //     }
                //     case "update":
                //     {
                //         await bot.SendMessage(query.Message!.Chat,$"Отправь данные пользователя в формате\nId\ntgId\nName\nactualCode\nRole\npaidUntile({DateTime.Now.ToString()})");
                //         userStates[query.Message!.Chat.Id] = "update";
                //         break;
                //     }
                // }
                // await bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
        }
}