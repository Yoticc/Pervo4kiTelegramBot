using Telegram.Bot.Types;

class Module
{
    public Module() => Dispatcher = new(this);

    public ModuleDispatcher Dispatcher;
    protected virtual async Task OnInit() => await Task.CompletedTask;
    protected virtual async Task OnMessage(TelegramInstance telegram, Update update, Message message) => await Task.CompletedTask;

    public class ModuleDispatcher(Module context)
    {
        public async Task InvokeOnInit() => await context.OnInit();
        public async Task InvokeOnMessage(TelegramInstance telegram, Update update, Message message) => await context.OnMessage(telegram, update, message);
    }
}