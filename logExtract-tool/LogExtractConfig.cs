namespace logExtract_tool
{
    public class LogExtractConfig
    {
        public string LogName { get; set; } = "";
        public string Source { get; set; } = "";
        public int[] LogLevels { get; set; } = System.Array.Empty<int>();
        public int Count { get; set; }
    }
}
