
using System.Text;
using CliWrap;
using CliWrap.Buffered;

public interface EventsExtractor
{
    IAsyncEnumerable<ExtractedEvent> ExtractEventsAsync();
}

public class EventLogExtractor : EventsExtractor
{
    public async IAsyncEnumerable<ExtractedEvent> ExtractEventsAsync()
    {
        var query = "SELECT TOP 20 TimeGenerated, BIT_AND(EventID, 0x3fffffff) as EventID, EventTypeName as Name, SourceName as Source, Strings, Message, Data FROM APPLICATION WHERE EventType = 1 OR EventType = 2 ORDER BY TimeGenerated DESC";
        var outputOption = "-o:csv";

        var stdOutBuffer = new StringBuilder();
        await Cli.Wrap(@"C:\Program Files (x86)\Log Parser 2.2\LogParser.exe")
            .WithArguments($"\"{query}\" {outputOption}")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .ExecuteAsync();

        using var reader = new StringReader(stdOutBuffer.ToString());
        var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            IgnoreBlankLines = true
        };
        using var csv = new CsvHelper.CsvReader(reader, config);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var extractedEvent = new ExtractedEvent
            {
                TimeGenerated = csv.TryGetField<DateTime>("TimeGenerated", out var dt) ? dt : DateTime.MinValue,
                EventID = csv.TryGetField<int>("EventID", out var eid) ? eid : 0,
                Name = csv.TryGetField<string>("Name", out var name) ? name ?? string.Empty : string.Empty,
                Source = csv.TryGetField<string>("Source", out var src) ? src ?? string.Empty : string.Empty,
                Strings = csv.TryGetField<string>("Strings", out var str) ? str ?? string.Empty : string.Empty,
                Message = csv.TryGetField<string>("Message", out var msg) ? msg ?? string.Empty : string.Empty,
                Data = csv.TryGetField<string>("Data", out var data) ? data ?? string.Empty : string.Empty
            };
            if (dt != DateTime.MinValue || eid != 0 || !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(src) || !string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(msg) || !string.IsNullOrEmpty(data))
            {
                yield return extractedEvent;
            }
            else
            {
                // LogParser also outputs statistics at the end that are not part of the CSV data, so we skip those lines
                continue;
            }
        }
    }
}
