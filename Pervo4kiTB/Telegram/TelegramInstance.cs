using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using System.Diagnostics.CodeAnalysis;

class TelegramInstance
{
    public TelegramInstance(string tokenFile)
    {
        Client = new(System.IO.File.ReadAllText(tokenFile));        
        StartHandling();
    }

    public readonly TelegramBotClient Client;
    [AllowNull] public User BotUser { get; private set; }

    readonly List<TelegramPlugin> plugins = [];

    public void AttachPlugin(TelegramPlugin plugin) => plugins.Add(plugin);

    async void StartHandling()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = 
            [
                UpdateType.Message,
                UpdateType.ChatMember
            ],
            ThrowPendingUpdates = true,            
        };

        using var cts = new CancellationTokenSource();

        Client.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);

        BotUser = await Client.GetMeAsync();
    }

    async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var plugin in plugins)
                await plugin.Dispatcher.InvokeOnUpdate(this, update);
        }
        catch (Exception ex) { Console.WriteLine(ex); }
        await Task.CompletedTask;
    }

    async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var message = error switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(message);
        await Task.CompletedTask;
    }
}