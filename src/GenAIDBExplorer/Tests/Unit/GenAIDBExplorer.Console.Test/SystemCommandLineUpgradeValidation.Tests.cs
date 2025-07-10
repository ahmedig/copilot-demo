using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Text;

namespace GenAIDBExplorer.Console.Test;

/// <summary>
/// Comprehensive tests to validate the System.CommandLine 2.0.0-beta5 upgrade
/// These tests ensure all CLI functionality works correctly with the new API
/// </summary>
[TestClass]
public class SystemCommandLineUpgradeValidationTests
{
    private const string ConsoleProjectPath = "../../../../GenAIDBExplorer.Console";
    private const int DefaultTimeoutMs = 30000;

    [TestMethod]
    public void RootCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("--help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("GenAI Database Explorer console application");
        result.Output.Should().Contain("Usage:");
        result.Output.Should().Contain("Commands:");
        result.Output.Should().Contain("init-project");
        result.Output.Should().Contain("data-dictionary");
        result.Output.Should().Contain("enrich-model");
        result.Output.Should().Contain("export-model");
        result.Output.Should().Contain("extract-model");
        result.Output.Should().Contain("query-model");
        result.Output.Should().Contain("show-object");
    }

    [TestMethod]
    public void InitProjectCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("init-project --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Initialize a GenAI Database Explorer project");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("The path to the GenAI Database Explorer project");
    }

    [TestMethod]
    public void ExtractModelCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("extract-model --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Extract a semantic model from a SQL database");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("--skipTables");
        result.Output.Should().Contain("--skipViews");
        result.Output.Should().Contain("--skipStoredProcedures");
    }

    [TestMethod]
    public void DataDictionaryCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("data-dictionary --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Process data dictionary files");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("Commands:");
        result.Output.Should().Contain("table");
    }

    [TestMethod]
    public void DataDictionaryTableCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("data-dictionary table --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Process table data dictionary files");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("-d, --source-path (REQUIRED)");
        result.Output.Should().Contain("-s, --schema");
        result.Output.Should().Contain("-n, --name");
        result.Output.Should().Contain("--show");
    }

    [TestMethod]
    public void EnrichModelCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("enrich-model --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Enrich an existing semantic model");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("--skipTables");
        result.Output.Should().Contain("--skipViews");
        result.Output.Should().Contain("--skipStoredProcedures");
        result.Output.Should().Contain("Commands:");
        result.Output.Should().Contain("table");
        result.Output.Should().Contain("view");
        result.Output.Should().Contain("storedprocedure");
    }

    [TestMethod]
    public void EnrichModelTableCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("enrich-model table --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Enrich a specific table");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("-s, --schema");
        result.Output.Should().Contain("-n, --name");
        result.Output.Should().Contain("--show");
    }

    [TestMethod]
    public void ExportModelCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("export-model --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Export the semantic model");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("-o, --outputFileName");
        result.Output.Should().Contain("-f, --fileType");
        result.Output.Should().Contain("-s, --splitFiles");
    }

    [TestMethod]
    public void QueryModelCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("query-model --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Answer questions based on the semantic model");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
    }

    [TestMethod]
    public void ShowObjectCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("show-object --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Show details of a semantic model object");
        result.Output.Should().Contain("Commands:");
        result.Output.Should().Contain("table");
        result.Output.Should().Contain("view");
        result.Output.Should().Contain("storedprocedure");
    }

    [TestMethod]
    public void ShowObjectTableCommand_Help_ShouldDisplayCorrectly()
    {
        // Act
        var result = RunCliCommand("show-object table --help");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().Contain("Show details of a table");
        result.Output.Should().Contain("-p, --project (REQUIRED)");
        result.Output.Should().Contain("-s, --schemaName (REQUIRED)");
        result.Output.Should().Contain("-n, --name (REQUIRED)");
    }

    [TestMethod]
    public void InitProjectCommand_RequiredParameter_ShouldShowError()
    {
        // Act
        var result = RunCliCommand("init-project");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
        result.Error.Should().Contain("Required");
    }

    [TestMethod]
    public void ExtractModelCommand_RequiredParameter_ShouldShowError()
    {
        // Act
        var result = RunCliCommand("extract-model");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
        result.Error.Should().Contain("Required");
    }

    [TestMethod]
    public void DataDictionaryTableCommand_RequiredParameters_ShouldShowError()
    {
        // Act
        var result = RunCliCommand("data-dictionary table");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
        result.Error.Should().Contain("Required");
    }

    [TestMethod]
    public void InvalidCommand_ShouldShowError()
    {
        // Act
        var result = RunCliCommand("invalid-command");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
    }

    [TestMethod]
    public void InvalidOption_ShouldShowError()
    {
        // Act
        var result = RunCliCommand("init-project --invalid-option");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().NotBe(0);
    }

    [TestMethod]
    public void VersionOption_ShouldDisplayVersion()
    {
        // Act
        var result = RunCliCommand("--version");

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        result.Output.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void AllCommands_OptionParsing_ShouldWork()
    {
        // Arrange
        var commands = new[]
        {
            "init-project --help",
            "extract-model --help",
            "data-dictionary --help",
            "data-dictionary table --help",
            "enrich-model --help",
            "enrich-model table --help",
            "enrich-model view --help",
            "enrich-model storedprocedure --help",
            "export-model --help",
            "query-model --help",
            "show-object --help",
            "show-object table --help",
            "show-object view --help",
            "show-object storedprocedure --help"
        };

        // Act & Assert
        foreach (var command in commands)
        {
            var result = RunCliCommand(command);
            result.Should().NotBeNull($"Command '{command}' should return a result");
            result.ExitCode.Should().Be(0, $"Command '{command}' should succeed");
            result.Output.Should().NotBeNullOrEmpty($"Command '{command}' should produce output");
        }
    }

    [TestMethod]
    public void Performance_HelpCommand_ShouldExecuteQuickly()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = RunCliCommand("--help");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Help command should execute within 5 seconds");
    }

    [TestMethod]
    public void Performance_SubcommandHelp_ShouldExecuteQuickly()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = RunCliCommand("enrich-model --help");
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.ExitCode.Should().Be(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Subcommand help should execute within 5 seconds");
    }

    /// <summary>
    /// Test result class to capture CLI execution results
    /// </summary>
    private class CliTestResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Helper method to run CLI commands and capture output
    /// </summary>
    /// <param name="arguments">Command line arguments to test</param>
    /// <returns>Test result with exit code and output</returns>
    private CliTestResult RunCliCommand(string arguments)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {ConsoleProjectPath} -- {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(typeof(SystemCommandLineUpgradeValidationTests).Assembly.Location) ?? Environment.CurrentDirectory
            };

            using var process = new Process { StartInfo = processStartInfo };
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var completed = process.WaitForExit(DefaultTimeoutMs);
            if (!completed)
            {
                process.Kill();
                throw new TimeoutException($"Command '{arguments}' timed out after {DefaultTimeoutMs}ms");
            }

            return new CliTestResult
            {
                ExitCode = process.ExitCode,
                Output = outputBuilder.ToString(),
                Error = errorBuilder.ToString()
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute command '{arguments}': {ex.Message}", ex);
        }
    }
}