using GenAIDBExplorer.Core.Models.SemanticModel;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace GenAIDBExplorer.Console.CommandHandlers;

/// <summary>
/// Defines the contract for a command handler using System.CommandLine 2.0.0-beta5 patterns.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for handling commands with specified options.
/// With System.CommandLine beta5, handlers use ParseResult parameters instead of individual typed parameters.
/// </remarks>
public interface ICommandHandler<TOptions> where TOptions : ICommandHandlerOptions
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="commandOptions">The command options that were provided to the command.</param>
    Task HandleAsync(TOptions commandOptions);
}
