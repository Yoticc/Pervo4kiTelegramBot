using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class MentionModule : Module
{
    static string MENTION_MESSAGE = "Радик зовёт!".PadRight(15);

    static Command manageMentionGroup = new(ValueType.String, ValueType.String, ValueType.ParamsString);
    static Command manageMention = new(ValueType.String, ValueType.String);
    static Command listMentionGroup = new(ValueType.String);

    protected override async Task OnMessage(TelegramInstance telegram, Update update, Message message)
    {
        var user = message.From!;
        var chat = message.Chat!;
        var text = message.Text!;

        if (text[0] == '#')
        {
            if (text.StartsWith("#all"))
            {
                if (Config.CheckAdministratorPrivilege(user.Id))
                {
                    const string BOUND = "################";
                    var input = InputFile.FromUri("https://media.tenor.com/tFfuxBS0VgwAAAAM/everyone.gif");
                    await telegram.Client.SendAnimationAsync(chat, input, caption: text.Length > 4 ? $"{BOUND}\n\n{text.Substring(5)}\n\n{BOUND}" : "");

                    var chunks = Config.Database.Users.Chunk(5);
                    foreach (var chunk in chunks)
                    {
                        var messageText = "";
                        for (var i = 0; i < chunk.Length; i++)
                        {
                            var chunkEntry = chunk[i];
                            var name = MENTION_MESSAGE.Substring(i * 3, (i + 1) == chunk.Length ? (MENTION_MESSAGE.Length - i * 3) : 3);
                            messageText += $"[{name}](tg://user?id={chunkEntry.Id})";
                        }

                        await telegram.Client.SendTextMessageAsync(chat, messageText, parseMode: ParseMode.Markdown);
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                }
                else
                {
                    await telegram.Client.SendTextMessageAsync(chat, "Не достоен 😂");
                }
            }
            else
            {
                var mentionName = new string(text.Skip(1).TakeWhile(c => c != ' ').ToArray());

                var messageGroup = Config.MentionGroups.Find(g => string.Equals(g.Name, mentionName, StringComparison.OrdinalIgnoreCase));
                if (messageGroup is null)
                    return;

                var chunks = messageGroup.Users.Chunk(5);
                foreach (var chunk in chunks)
                {
                    await telegram.Client.SendTextMessageAsync(chat, string.Join(' ', chunk));
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            }
        }
        else
        {
            var parsedText = ParsedText.Parse(message.Text!);
            if (listMentionGroup.IsValid(parsedText))
            {
                var args = listMentionGroup.Parse(parsedText);
                if (args[0] as string is "listMentionGroups")
                    await telegram.Client.SendTextMessageAsync(message.Chat, $"Группы: {string.Join(", ", Config.MentionGroups.Select(g => g.Name))}");
            }
            else if (manageMention.IsValid(parsedText))
            {
                var args = manageMention.Parse(parsedText);
                var groupName = (args[1] as string)!;

                if (args[0] as string is "listMentionUsers")
                {
                    if (args.Length - 1 == 0)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, "Укажите название группы");
                        return;
                    }

                    var group = Config.MentionGroups.Find(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase));
                    if (group is null)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, $"Не удалось найти группу {groupName}");
                        return;
                    }

                    await telegram.Client.SendTextMessageAsync(message.Chat, $"Участники: {string.Join(' ', group.Users.Select(u => u.Substring(1)))}");
                }
                else if (args[0] as string is "addMentionGroup")
                {
                    if (Config.MentionGroups.Find(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase)) is not null)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, $"Группа {groupName} уже существует");
                        return;
                    }

                    Config.MentionGroups.Add(new MentionGroup(groupName));
                    await telegram.Client.SendTextMessageAsync(message.Chat, "Группа успешно добавлена");
                }
                else if (args[0] as string is "removeMentionGroup")
                {
                    var index = Config.MentionGroups.FindIndex(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase));
                    if (index == -1)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, $"Группы {groupName} не существует");
                        return;
                    }

                    Config.MentionGroups.RemoveAt(index);
                    await telegram.Client.SendTextMessageAsync(message.Chat, "Группа успешно удалена");
                }
            }
            else if (manageMentionGroup.IsValid(parsedText))
            {
                var args = manageMentionGroup.Parse(parsedText);
                if (args[0] as string is "addMentionUser")
                {
                    if (args.Length - 1 == 0)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, "Укажите название группы");
                        return;
                    }

                    if (args.Length - 2 == 0)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, "Упомяните участников группы через пробел");
                        return;
                    }

                    var groupName = args[1] as string;
                    var group = Config.MentionGroups.Find(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase));
                    if (group is null)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, $"Не удалось найти группу {groupName}");
                        return;
                    }

                    var users = args.Skip(2).ToList();

                    foreach (var rawAnUser in users)
                    {
                        var anUser = (rawAnUser as string)!;

                        if (anUser[0] != '@')
                            continue;

                        if (group.Users.Find(u => string.Equals(anUser, u)) is not null)
                            continue;

                        group.Users.Add(anUser);
                    }

                    await telegram.Client.SendTextMessageAsync(message.Chat, "Успешно");
                }
                if (args[0] as string is "removeMentionUser")
                {
                    if (args.Length - 1 == 0)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, "Укажите название группы");
                        return;
                    }

                    if (args.Length - 2 == 0)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, "Упомяните участников группы через пробел");
                        return;
                    }

                    var groupName = args[1] as string;
                    var group = Config.MentionGroups.Find(g => string.Equals(g.Name, groupName, StringComparison.OrdinalIgnoreCase));
                    if (group is null)
                    {
                        await telegram.Client.SendTextMessageAsync(message.Chat, $"Не удалось найти группу {groupName}");
                        return;
                    }

                    var users = args.Skip(2).ToList();

                    foreach (var rawAnUser in users)
                    {
                        var anUser = (rawAnUser as string)!;

                        if (anUser[0] != '@')
                            continue;

                        if (group.Users.Find(u => string.Equals(anUser, u)) is null)
                            continue;

                        group.Users.Remove(anUser);
                    }

                    await telegram.Client.SendTextMessageAsync(message.Chat, "Успешно");
                }
            }
        }

        await Task.CompletedTask;
    }
}