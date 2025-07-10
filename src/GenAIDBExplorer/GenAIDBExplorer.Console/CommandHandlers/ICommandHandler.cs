using GenAIDBExplorer.Core.Models.SemanticModel;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace GenAIDBExplorer.Console.CommandHandlers;

/// <summary>
/// Defines the contract for a command handler.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for handling commands with specified options.
/// </remarks>
public interface ICommandHandler<TOptions> where TOptions : ICommandHandlerOptions
{
    /// <summary>
    /// Handles the command with strongly-typed options.
    /// </summary>
    /// <param name="commandOptions">The command options that were provided to the command.</param>
    Task HandleAsync(TOptions commandOptions);

    /// <summary>
    /// Handles the command using ParseResult for System.CommandLine 2.0.0-beta5 compatibility.
    /// </summary>
    /// <param name="parseResult">The parse result containing the parsed command-line arguments.</param>
    Task HandleAsync(ParseResult parseResult);
}
