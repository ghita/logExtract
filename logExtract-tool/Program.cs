using System.CommandLine;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using logExtract_tool;

var builder = new ConfigurationBuilder()
    .AddEnvironmentVariables();

var configuration = builder.Build();

var inputConfigOption = new Option<string?>(["-f", "--file"], () => null, "The JSON file that describes the log extraction configuration.");
var outputFileOption = new Option<string?>(["-o", "--output"], () => null, "The output file name and path. Default to console output if not specified.");

var rootCommand = new RootCommand("A tool to extract logs from large log files based on a JSON configuration.")
{
    inputConfigOption,
    outputFileOption
};

rootCommand.AddValidator(r =>
{
    var inputConfig = r.GetValueForOption(inputConfigOption);
    if (string.IsNullOrEmpty(inputConfig))
    {
        r.ErrorMessage = "The -f or --file option is required.";
    }
});

rootCommand.SetHandler(RunAsync, inputConfigOption, outputFileOption);

await rootCommand.InvokeAsync(args);

async Task RunAsync(string? inputConfig, string? outputFile)
{
    // Load and parse the JSON configuration file
    Console.WriteLine($"Loading configuration from: {inputConfig}");
    await using var configFileStream = new FileStream(inputConfig!, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

    var config = await JsonSerializer.DeserializeAsync<LogExtractConfig>(configFileStream);
    if (config == null)
    {
        Console.Error.WriteLine("Failed to parse configuration file.");
        return;
    }

    // Example usage
    Console.WriteLine($"LogName: {config.LogName}");
    Console.WriteLine($"Source: {config.Source}");
    Console.WriteLine($"LogLevels: {string.Join(",", config.LogLevels)}");
    Console.WriteLine($"Count: {config.Count}");
}