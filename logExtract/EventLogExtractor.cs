
using System.Text;
using CliWrap;
using CliWrap.Buffered;
using logExtract;

public interface EventsExtractor
{
    IAsyncEnumerable<ExtractedEvent> ExtractEventsAsync();
}

public class EventLogExtractor : EventsExtractor
{
    public async IAsyncEnumerable<ExtractedEvent> ExtractEventsAsync()
    {
        var query = "SELECT TOP 30 TimeGenerated, BIT_AND(EventID, 0x3fffffff) as EventID, EventTypeName as Name, SourceName as Source, Strings, Message, Data FROM APPLICATION WHERE EventType = 1 OR EventType = 2 ORDER BY TimeGenerated DESC";
        var outputOption = "-o:csv";

        var stdOutBuffer = new StringBuilder();
        await Cli.Wrap(@"C:\Program Files (x86)\Log Parser 2.2\LogParser.exe")
            .WithArguments($"\"{query}\" {outputOption}")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .ExecuteAsync();

        // Use CSVExtract to parse CSV output
        var csvBytes = Encoding.UTF8.GetBytes(stdOutBuffer.ToString());
        using var csvStream = new MemoryStream(csvBytes);
        var csvExtract = new CSVExtract(csvStream, hasHeader: true, encoding: Encoding.UTF8);
        foreach (var row in csvExtract.Parse())
        {
            var extractedEvent = new ExtractedEvent
            {
                TimeGenerated = DateTime.TryParse(row.TryGetValue("TimeGenerated", out var dtStr) ? dtStr : null, out var dt) ? dt : DateTime.MinValue,
                EventID = int.TryParse(row.TryGetValue("EventID", out var eidStr) ? eidStr : null, out var eid) ? eid : 0,
                Name = row.TryGetValue("Name", out var name) ? name ?? string.Empty : string.Empty,
                Source = row.TryGetValue("Source", out var source) ? source ?? string.Empty : string.Empty,
                Strings = row.TryGetValue("Strings", out var strings) ? strings ?? string.Empty : string.Empty,
                Message = row.TryGetValue("Message", out var message) ? message ?? string.Empty : string.Empty,
                Data = row.TryGetValue("Data", out var data) ? data ?? string.Empty : string.Empty
            };
            if (extractedEvent.TimeGenerated != DateTime.MinValue || extractedEvent.EventID != 0 || !string.IsNullOrEmpty(extractedEvent.Name) || !string.IsNullOrEmpty(extractedEvent.Source) || !string.IsNullOrEmpty(extractedEvent.Strings) || !string.IsNullOrEmpty(extractedEvent.Message) || !string.IsNullOrEmpty(extractedEvent.Data))
            {
                yield return extractedEvent;
            }
        }
    }
}
