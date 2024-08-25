using Telegram.Bot.Types;

class DatabaseProvider : Module
{
    protected override async Task OnMessage(TelegramInstance telegram, Update update, Message message)
    {
        Config.Database.HandleUser(message.From!);
        await Task.CompletedTask;
    }
}