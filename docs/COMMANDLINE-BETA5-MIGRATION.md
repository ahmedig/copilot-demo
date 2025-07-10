# System.CommandLine 2.0.0-beta5 Migration Guide

This document outlines the changes made to support System.CommandLine 2.0.0-beta5 in the CommandHandler base class and provides guidance for developers working with command handlers.

## Overview

The CommandHandler base class has been updated to support both the legacy pattern (using strongly-typed options) and the new System.CommandLine 2.0.0-beta5 pattern (using ParseResult).

## Key Changes

### 1. ICommandHandler Interface

The interface now includes two overloads:

```csharp
public interface ICommandHandler<TOptions> where TOptions : ICommandHandlerOptions
{
    // Legacy method - continues to work as before
    Task HandleAsync(TOptions commandOptions);
    
    // New method for beta5 compatibility
    Task HandleAsync(ParseResult parseResult);
}
```

### 2. CommandHandler Base Class

The base class provides:

- **Backward Compatibility**: Existing `HandleAsync(TOptions)` method continues to work
- **Forward Compatibility**: New `HandleAsync(ParseResult)` method that delegates to the legacy method
- **Extraction Framework**: `ExtractCommandOptions(ParseResult)` method for converting ParseResult to strongly-typed options
- **Utility Methods**: Helper methods for extracting values from ParseResult

### 3. Utility Methods

The base class provides several utility methods for working with ParseResult:

```csharp
// Extract option value by Option<T> reference
protected static T GetOptionValue<T>(ParseResult parseResult, Option<T> option)

// Extract option value by name
protected static T GetOptionValue<T>(ParseResult parseResult, string optionName)

// Check if option was provided
protected static bool HasOption(ParseResult parseResult, Option option)
```

## Implementation Pattern

### For Command Handlers

Each command handler should implement the `ExtractCommandOptions` method:

```csharp
public class MyCommandHandler : CommandHandler<MyCommandHandlerOptions>
{
    // ... constructor and existing HandleAsync(MyCommandHandlerOptions) method ...

    protected override MyCommandHandlerOptions ExtractCommandOptions(ParseResult parseResult)
    {
        var projectPath = GetOptionValue<DirectoryInfo>(parseResult, "--project");
        var someFlag = GetOptionValue<bool>(parseResult, "--some-flag");
        
        return new MyCommandHandlerOptions(projectPath, someFlag);
    }
}
```

### For SetupCommand Methods (Future Migration)

When migrating SetupCommand methods to beta5 (in issues #18-#24), the pattern will change from:

**Beta4 Pattern:**
```csharp
command.SetHandler(async (DirectoryInfo projectPath, bool flag) =>
{
    var handler = host.Services.GetRequiredService<MyCommandHandler>();
    var options = new MyCommandHandlerOptions(projectPath, flag);
    await handler.HandleAsync(options);
}, projectPathOption, flagOption);
```

**Beta5 Pattern:**
```csharp
command.SetAction(async (parseResult) =>
{
    var handler = host.Services.GetRequiredService<MyCommandHandler>();
    await handler.HandleAsync(parseResult);
});
```

## Migration Status

### Completed
- ✅ ICommandHandler interface updated with ParseResult overload
- ✅ CommandHandler base class updated with extraction framework
- ✅ InitProjectCommandHandler updated with ExtractCommandOptions
- ✅ ExtractModelCommandHandler updated with ExtractCommandOptions  
- ✅ QueryModelCommandHandler updated with ExtractCommandOptions

### Pending (Future Issues)
- ⏳ DataDictionaryCommandHandler (Issue #18)
- ⏳ EnrichModelCommandHandler (Issue #19)
- ⏳ ExportModelCommandHandler (Issue #20)
- ⏳ ShowObjectCommandHandler (Issue #21)
- ⏳ Update SetupCommand methods to use SetAction (Issues #22-#24)

## Benefits

1. **Zero Breaking Changes**: Existing code continues to work unchanged
2. **Future Ready**: New ParseResult pattern is ready for beta5 API usage
3. **Consistent Pattern**: All command handlers follow the same extraction pattern
4. **Type Safety**: Strongly-typed options are preserved throughout the application
5. **Testability**: Both patterns can be easily unit tested

## Testing

The new functionality includes test coverage:

```csharp
[TestMethod]
public async Task HandleAsync_WithParseResult_ShouldWork()
{
    // Arrange
    var projectOption = new Option<DirectoryInfo>("--project", "Project path");
    var rootCommand = new RootCommand();
    rootCommand.AddOption(projectOption);
    var parseResult = rootCommand.Parse(new[] { "--project", "/path/to/project" });

    // Act
    await handler.HandleAsync(parseResult);

    // Assert - verify the command executed correctly
}
```

## Next Steps

1. Complete the migration of remaining command handlers
2. Update SetupCommand methods to use SetAction (depends on issues #15 and #16)
3. Update Program.cs command registration (issue #16)
4. Update package reference to beta5 (issue #15)