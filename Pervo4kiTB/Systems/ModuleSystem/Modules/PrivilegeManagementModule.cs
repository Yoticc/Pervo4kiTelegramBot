using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class PrivilegeManagementModule : Module
{
    static Command manageAdmins = new(ValueType.String, ValueType.ParamsLong);

    protected override async Task OnMessage(TelegramInstance telegram, Update update, Message message)
    {
        var user = message.From!;

        if (!Config.CheckAdministratorPrivilege(user.Id))
            return;

        var parsedText = ParsedText.Parse(message.Text!);
        if (manageAdmins.IsValid(parsedText))
        {
            var args = manageAdmins.Parse(parsedText);
            if (args[0] as string is "addAdmin")
            {
                if (args.Length - 1 == 0)
                {
                    await telegram.Client.SendTextMessageAsync(message.Chat, "Укажите участников чата через пробел по их ID");
                    return;
                }

                var ids = args.Skip(1).Select(a => (long)a).ToList();
                foreach (var id in ids)
                    if (!Config.Administration.Contains(id))
                        Config.Administration.Add(id);

                await telegram.Client.SendTextMessageAsync(message.Chat, "Администрация успешно добавлена");
            }
            else if (args[0] as string is "removeAdmin")
            {
                if (args.Length - 1 == 0)
                {
                    await telegram.Client.SendTextMessageAsync(message.Chat, "Укажите участников чата через пробел по их ID");
                    return;
                }

                var ids = args.Skip(1).Select(a => (long)a).ToList();
                foreach (var id in ids)
                    Config.Administration.Remove(id);

                await telegram.Client.SendTextMessageAsync(message.Chat, "Администрация успешно добавлена");
            }
            else if (args[0] as string is "listAdmins")
                await telegram.Client.SendTextMessageAsync(
                    chatId: message.Chat, 
                    text: $"Администрация:\n {string.Join("\n ", Config.Administration.Select(adm => $"[{Config.Database.GetNameFor(adm)}](tg://user?id={adm})"))}", 
                    parseMode: ParseMode.Markdown, 
                    disableNotification: true
                );
        }
    }
}