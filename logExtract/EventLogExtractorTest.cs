using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Versioning;

namespace LogExtract
{

    [SupportedOSPlatform("windows")]
    public class EventLogExtractor
    {
        public string LogName { get; set; } = "Application";
        public string Source { get; set; }

        public EventLogExtractor(string source, string logName = "Application")
        {
            Source = source;
            LogName = logName;
        }

        public List<EventRecord> GetEntries()
        {
            var entries = new List<EventRecord>();
            string query = $"<QueryList><Query Id=\"0\" Path=\"{LogName}\"><Select Path=\"{LogName}\">*[System/Provider/@Name='{Source}']</Select></Query></QueryList>";
            using (var reader = new EventLogReader(new EventLogQuery(LogName, PathType.LogName, query)))
            {
                for (EventRecord eventInstance = reader.ReadEvent(); eventInstance != null; eventInstance = reader.ReadEvent())
                {
                    entries.Add(eventInstance);
                }
            }
            return entries;
        }
    }
}
