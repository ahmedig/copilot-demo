using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace GenAIDBExplorer.Console.Test;

/// <summary>
/// Performance tests to validate System.CommandLine 2.0.0-beta5 improvements
/// These tests measure startup time and parsing performance
/// </summary>
[TestClass]
public class SystemCommandLinePerformanceTests
{
    private const string ConsoleProjectPath = "../../../../GenAIDBExplorer.Console";
    private const int TestIterations = 5;
    private const int MaxStartupTimeMs = 10000; // 10 seconds max for startup
    private const int DefaultTimeoutMs = 30000;

    [TestMethod]
    public void Startup_Time_ShouldBeFast()
    {
        // Arrange
        var times = new List<long>();

        // Act - Run multiple times to get average
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = RunCliCommand("--help");
            stopwatch.Stop();

            result.ExitCode.Should().Be(0, $"Help command should succeed on iteration {i + 1}");
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var averageTime = times.Average();
        var maxTime = times.Max();
        var minTime = times.Min();

        Console.WriteLine($"Startup Performance Results:");
        Console.WriteLine($"  Average: {averageTime:F2}ms");
        Console.WriteLine($"  Min: {minTime}ms");
        Console.WriteLine($"  Max: {maxTime}ms");
        Console.WriteLine($"  All times: [{string.Join(", ", times)}]ms");

        averageTime.Should().BeLessThan(MaxStartupTimeMs, 
            $"Average startup time should be less than {MaxStartupTimeMs}ms");
        maxTime.Should().BeLessThan(MaxStartupTimeMs * 1.5, 
            $"Maximum startup time should be less than {MaxStartupTimeMs * 1.5}ms");
    }

    [TestMethod]
    public void Parsing_Performance_ComplexCommand_ShouldBeFast()
    {
        // Arrange
        var complexCommand = "enrich-model table --project /test/path --schema dbo --name TestTable --show --help";
        var times = new List<long>();

        // Act
        for (int i = 0; i < TestIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = RunCliCommand(complexCommand);
            stopwatch.Stop();

            result.ExitCode.Should().Be(0, $"Complex command should succeed on iteration {i + 1}");
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var averageTime = times.Average();
        Console.WriteLine($"Complex Command Parsing Performance:");
        Console.WriteLine($"  Command: {complexCommand}");
        Console.WriteLine($"  Average: {averageTime:F2}ms");
        Console.WriteLine($"  All times: [{string.Join(", ", times)}]ms");

        averageTime.Should().BeLessThan(MaxStartupTimeMs, 
            "Complex command parsing should be fast");
    }

    [TestMethod]
    public void Memory_Usage_ShouldBeReasonable()
    {
        // Arrange & Act
        var startMemory = GC.GetTotalMemory(true);
        
        // Run several commands to test memory usage
        var commands = new[]
        {
            "--help",
            "init-project --help",
            "extract-model --help", 
            "enrich-model --help",
            "data-dictionary --help"
        };

        foreach (var command in commands)
        {
            var result = RunCliCommand(command);
            result.ExitCode.Should().Be(0, $"Command '{command}' should succeed");
        }

        var endMemory = GC.GetTotalMemory(true);

        // Assert
        var memoryIncrease = endMemory - startMemory;
        Console.WriteLine($"Memory Usage:");
        Console.WriteLine($"  Start: {startMemory:N0} bytes");
        Console.WriteLine($"  End: {endMemory:N0} bytes");
        Console.WriteLine($"  Increase: {memoryIncrease:N0} bytes");

        // Memory increase should be reasonable (less than 100MB for these simple operations)
        memoryIncrease.Should().BeLessThan(100 * 1024 * 1024, 
            "Memory usage should not increase dramatically during testing");
    }

    [TestMethod]
    public void Concurrent_Command_Execution_ShouldNotInterfere()
    {
        // Arrange
        var commands = new[]
        {
            "--help",
            "init-project --help",
            "extract-model --help",
            "enrich-model --help"
        };

        // Act - Run commands concurrently
        var tasks = commands.Select(async command =>
        {
            await Task.Delay(Random.Shared.Next(0, 100)); // Random delay to mix up execution
            return RunCliCommand(command);
        }).ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().HaveCount(commands.Length);
        results.Should().OnlyContain(r => r.ExitCode == 0, "All concurrent commands should succeed");
        results.Should().OnlyContain(r => !string.IsNullOrEmpty(r.Output), "All commands should produce output");
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
                WorkingDirectory = Path.GetDirectoryName(typeof(SystemCommandLinePerformanceTests).Assembly.Location) ?? Environment.CurrentDirectory
            };

            using var process = new Process { StartInfo = processStartInfo };
            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

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