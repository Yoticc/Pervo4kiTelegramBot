using Telegram.Bot.Types;

class TelegramPlugin
{
    public TelegramPlugin()
    {
        Dispatcher = new(this);
    }

    public readonly TelegramPluginDispatcher Dispatcher;

    protected virtual async Task OnUpdate(TelegramInstance telegram, Update update) => await Task.CompletedTask;

    public class TelegramPluginDispatcher(TelegramPlugin context)
    {
        public async Task InvokeOnUpdate(TelegramInstance telegram, Update update) => await context.OnUpdate(telegram, update);
    }
}