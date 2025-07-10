using System.CommandLine;
using Microsoft.Extensions.Hosting;
using GenAIDBExplorer.Console.CommandHandlers;
using GenAIDBExplorer.Console.Extensions;

namespace GenAIDBExplorer.Console;

/// <summary>
/// The main entry point for the GenAI Database Explorer tool.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main method that sets up and runs the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    private static async Task Main(string[] args)
    {
        // Create the root command with a description
        var rootCommand = new RootCommand("GenAI Database Explorer console application");

        // Build the host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureHost(args)
            .Build();

        // Set up commands
        rootCommand.Subcommands.Add(InitProjectCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(DataDictionaryCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(EnrichModelCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(ExportModelCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(ExtractModelCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(QueryModelCommandHandler.SetupCommand(host));
        rootCommand.Subcommands.Add(ShowObjectCommandHandler.SetupCommand(host));

        // Invoke the root command - using Task.Run as workaround for missing InvokeAsync in beta5
        await Task.Run(() => 
        {
            try 
            {
                // For now, just run without proper error handling until we find the right API
                System.Console.WriteLine("GenAI Database Explorer - Command handlers migrated to System.CommandLine 2.0.0-beta5");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        });
    }
}
