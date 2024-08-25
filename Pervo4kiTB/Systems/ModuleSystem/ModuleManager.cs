using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class ModuleManager : TelegramPlugin
{
    public ModuleManager() : this((List<Module>)[]) { }
    public ModuleManager(params Module[] modules) : this(modules.ToList()) { }
    public ModuleManager(List<Module> modules)
    {
        this.modules = modules;

        foreach (var module in modules)
            module.Dispatcher.InvokeOnInit().GetAwaiter().GetResult();
    }

    readonly List<Module> modules;

    protected override async Task OnUpdate(TelegramInstance telegram, Update update)
    {
        if (update.Type == UpdateType.Message)
        {
            try
            {
                if (update.Message is null)
                    return;

                if (update.Message.Text is null)
                    return;

                await OnMessage(telegram, update, update.Message);
            }
            catch { }
        }
    }

    async Task OnMessage(TelegramInstance telegram, Update update, Message message)
    {
        foreach (var module in modules)
            await module.Dispatcher.InvokeOnMessage(telegram, update, message);
    }
}