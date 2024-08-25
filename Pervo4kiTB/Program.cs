var storage = new FileStorage(@"C:\Data\pervo4kiBot");
var telegram = new TelegramInstance(storage.GetPath("token.txt"));
var modules = new ModuleManager(
    new DatabaseProvider(),
    new HelpModule(),
    new PrivilegeManagementModule(),
    new MentionModule()
);
telegram.AttachPlugin(modules);

var configManager = new ConfigManager<BotConfig>(storage.GetPath("config.json"));
Config = configManager.Config;

await Task.Delay(-1);