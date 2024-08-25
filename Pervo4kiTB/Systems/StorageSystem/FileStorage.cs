class FileStorage
{
    public FileStorage(string root) => Directory.CreateDirectory(Root = root);

    public readonly string Root;

    public string GetPath(params string[] parts) => Path.Combine([Root, .. parts]);
}