// Example: System.CommandLine 2.0.0-beta5 Migration Pattern
// This file demonstrates how the updated CommandHandler base class
// will work with the new beta5 API patterns.

using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Hosting;

namespace GenAIDBExplorer.Console.Examples;

/// <summary>
/// Example showing how SetupCommand methods will work with beta5 API
/// </summary>
public static class Beta5MigrationExample
{
    /// <summary>
    /// Example of how the old beta4 SetupCommand method looked
    /// </summary>
    public static Command SetupCommand_Beta4_Pattern(IHost host)
    {
        var projectPathOption = new Option<DirectoryInfo>(
            aliases: ["--project", "-p"],
            description: "The path to the GenAI Database Explorer project."
        )
        {
            IsRequired = true  // Beta4 syntax
        };

        var command = new Command("example", "Example command");
        command.AddOption(projectPathOption);  // Beta4 syntax
        
        // Beta4 handler pattern
        command.SetHandler(async (DirectoryInfo projectPath) =>
        {
            var handler = host.Services.GetRequiredService<ExampleCommandHandler>();
            var options = new ExampleCommandHandlerOptions(projectPath);
            await handler.HandleAsync(options);  // Uses TOptions
        }, projectPathOption);

        return command;
    }

    /// <summary>
    /// Example of how the new beta5 SetupCommand method will look
    /// </summary>
    public static Command SetupCommand_Beta5_Pattern(IHost host)
    {
        var projectPathOption = new Option<DirectoryInfo>("--project", "-p")
        {
            Description = "The path to the GenAI Database Explorer project.",
            Required = true  // Beta5 syntax
        };

        var command = new Command("example", "Example command");
        command.Options.Add(projectPathOption);  // Beta5 syntax
        
        // Beta5 handler pattern
        command.SetAction(async (parseResult) =>
        {
            var handler = host.Services.GetRequiredService<ExampleCommandHandler>();
            await handler.HandleAsync(parseResult);  // Uses ParseResult - NEW!
        });

        return command;
    }
}

/// <summary>
/// Example command handler showing both old and new patterns
/// </summary>
public class ExampleCommandHandler : CommandHandler<ExampleCommandHandlerOptions>
{
    // Constructor would go here...

    /// <summary>
    /// Legacy method - continues to work as before
    /// </summary>
    public override async Task HandleAsync(ExampleCommandHandlerOptions commandOptions)
    {
        // Original implementation stays the same
        var projectPath = commandOptions.ProjectPath;
        // ... handle command logic
        await Task.CompletedTask;
    }

    /// <summary>
    /// New method for beta5 compatibility - automatically provided by base class
    /// The base class HandleAsync(ParseResult) calls this method to extract options
    /// </summary>
    protected override ExampleCommandHandlerOptions ExtractCommandOptions(ParseResult parseResult)
    {
        // Use utility methods from base class to extract values
        var projectPath = GetOptionValue<DirectoryInfo>(parseResult, "--project");
        return new ExampleCommandHandlerOptions(projectPath);
    }
}

/// <summary>
/// Options class - no changes needed
/// </summary>
public class ExampleCommandHandlerOptions : ICommandHandlerOptions
{
    public ExampleCommandHandlerOptions(DirectoryInfo projectPath)
    {
        ProjectPath = projectPath;
    }

    public DirectoryInfo ProjectPath { get; }
}

/*
MIGRATION SUMMARY:

WHAT CHANGES:
1. SetupCommand methods update to use:
   - Required = true instead of IsRequired = true
   - Options.Add() instead of AddOption()  
   - SetAction() instead of SetHandler()
   - ParseResult parameter instead of individual parameters

2. Command handlers implement:
   - ExtractCommandOptions(ParseResult) method
   - Use GetOptionValue<T>() utility methods

WHAT STAYS THE SAME:
1. HandleAsync(TOptions) method - no changes
2. Options classes - no changes  
3. Command logic - no changes
4. Dependency injection - no changes
5. Tests - existing tests continue to work

BENEFITS:
- Zero breaking changes to existing code
- Forward compatibility with beta5
- Type safety preserved
- Easy migration path
- Better performance with beta5
*/