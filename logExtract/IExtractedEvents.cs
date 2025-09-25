using LogExtract;
using System;
using System.Linq;

public interface IExtractedEvents
{
    DateTime TimeGenerated { get; }
    string Name { get; }
    string Source { get; }
    string Strings { get; }
    string Data { get; }
    string Message { get; }
}
