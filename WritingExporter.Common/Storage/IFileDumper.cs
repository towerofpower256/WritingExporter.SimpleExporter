namespace WritingExporter.Common.Storage
{
    public interface IFileDumper
    {
        string DumpFile(string name, string content);
    }
}