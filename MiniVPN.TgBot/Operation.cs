using Telegram.Bot;
using Telegram.Bot.Types;
using UserChecker.Server.Model;

namespace UserChecker.ClientTgBot;

public delegate Task PressButtonHandler(Message msgButton);
public delegate Task SendMessageAfterOperationHandler(Message msg);

public class Operation(string name, RoleDB roleDb, PressButtonHandler onPressButton, SendMessageAfterOperationHandler onSendMassageAfterOperation)
{
    public readonly string Name = name;
    public readonly RoleDB NeedRoleDb = roleDb;
    public event PressButtonHandler PressButtonEvent = onPressButton;
    public event SendMessageAfterOperationHandler SendMassageAfterOperationEvent = onSendMassageAfterOperation;

    public Task OnPressButton(Message msgbutton)
    {
        PressButtonEvent?.Invoke(msgbutton);
        return Task.CompletedTask;
    }

    public Task OnSendMassageAfterOperation (Message msg)
    {
        SendMassageAfterOperationEvent?.Invoke(msg);
        return Task.CompletedTask;
    }
}