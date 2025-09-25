class Program
{
    static async Task Main(string[] args)
    {
        var extractor = new EventLogExtractor();
        var events = extractor.ExtractEventsAsync();
        int lineCount = 0;
        await foreach (var evt in events)
        {
            Console.WriteLine($"Event ID: {evt.EventID}, Time Generated: {evt.TimeGenerated}, Name: {evt.Name}, Source: {evt.Source}, Message: {evt.Message}");
            lineCount++;
        }
        Console.WriteLine($"Total lines: {lineCount}");
    }
}
