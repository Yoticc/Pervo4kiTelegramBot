using Newtonsoft.Json;

class ConfigManager<T> : IDisposable where T : new()
{
    public ConfigManager(string path)
    {
        FilePath = path;
        Config = Load();

        savingThread = new(StartSaving);
        savingThread.Start();
    }

    public readonly string FilePath;
    public readonly T Config;
    public TimeSpan Delay = TimeSpan.FromSeconds(10);

    Thread savingThread;

    void StartSaving()
    {
        while (true)
        {
            Save();
            Thread.Sleep(Delay);
        }
    }

    void Save() => File.WriteAllText(FilePath, JsonConvert.SerializeObject(Config));

    T Load()
    {
        if (!File.Exists(FilePath))
            return new T();

        var instance = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
        return instance ?? new T();
    }

    bool isDisposed;
    public void Dispose() => isDisposed = true;
    ~ConfigManager() => Dispose();
}