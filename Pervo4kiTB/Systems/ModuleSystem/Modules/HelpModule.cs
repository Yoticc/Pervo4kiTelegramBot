using Telegram.Bot;
using Telegram.Bot.Types;

class HelpModule : Module
{
    protected override async Task OnMessage(TelegramInstance telegram, Update update, Message message)
    {
        if (string.Equals(message.Text, "help", StringComparison.OrdinalIgnoreCase))
        {
            await telegram.Client.SendTextMessageAsync(message.Chat,
@"
help - Показать это сообщение

addAdmin: ID[] — Добавить администрацию в бота
removeAdmin: ID[] — Удалить администрацию из бота
listAdmins — Показать админиматрцию

addMentionGroup GroupName — Добавить новую группу для упоминаний
removeMentionGroup GroupName — Удалить группу для упоминаний
addMentionUser: GroupName @username[] — Добавить участников для упоминания в группу
removeMentionUser: GroupName @username[] — Добавить участников для упоминания в группу
listMentionUsers: GroupName — Показать участников группы
listMentionGroups — Показать группы для упоминания

#all — Упомянуть всех
#{mention_name} — Упомянуть группу
"
        );
        }

        await Task.CompletedTask;
    }
}