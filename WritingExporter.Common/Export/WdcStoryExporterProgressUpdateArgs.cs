namespace WritingExporter.Common.Export
{
    public class WdcStoryExporterProgressUpdateArgs
    {
        public int ProgressValue { get; set; }
        public int ProgressMax { get; set; }
        public string Message { get; set; }
    }
}