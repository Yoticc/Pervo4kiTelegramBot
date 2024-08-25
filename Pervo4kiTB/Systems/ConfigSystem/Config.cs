using Telegram.Bot.Types;

class BotConfig
{
    public const long PermanentAdministrator = 1286981709;
    public List<long> Administration = [];
    public Database Database = new();
    public List<MentionGroup> MentionGroups = [];

    public bool CheckAdministratorPrivilege(long id) => Administration.Contains(id);
}

record Database
{
    public List<User> Users = [];

    public void HandleUser(User user)
    {
        if (user.Username is not null && user.Username.EndsWith("bot", StringComparison.OrdinalIgnoreCase))
            return;

        var existingUser = Users.Find(u => u.Id == user.Id);
        if (existingUser is null)
            Users.Add(user);
        else
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
        }
    }

    public string GetNameFor(long id)
    {
        var user = Users.Find(user => user.Id == id);

        if (user is null)
            return id.ToString();

        return $"{user.FirstName} {user.FirstName}";
    }
}

record MentionGroup(string Name)
{
    public List<string> Users = [];
}