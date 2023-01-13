//
//  XUnitLogger.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Helpers;

/// <summary>
/// X unit logger.
/// </summary>
internal class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider;

    /// <summary>
    /// Creates a logger for the given test output.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <returns>A logger.</returns>
    public static ILogger CreateLogger(ITestOutputHelper testOutputHelper)
        => new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), string.Empty);

    /// <summary>
    /// Creates a logger for the given test output.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <typeparam name="T">The type to create logger for.</typeparam>
    /// <returns>A logger.</returns>
    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper)
        => new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());

    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLogger"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <param name="scopeProvider">The scope provider.</param>
    /// <param name="categoryName">The category name.</param>
    public XUnitLogger
        (ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName)
    {
        _testOutputHelper = testOutputHelper;
        _scopeProvider = scopeProvider;
        _categoryName = categoryName;
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None;

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    /// <inheritdoc/>
    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var sb = new StringBuilder();
        sb.Append(GetLogLevelString(logLevel))
            .Append(" [").Append(_categoryName).Append("] ")
            .Append(formatter(state, exception));

        if (exception != null)
        {
            sb.Append('\n').Append(exception);
        }

        // Append scopes
        _scopeProvider.ForEachScope
        (
            (scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            },
            sb
        );

        _testOutputHelper.WriteLine(sb.ToString());
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}

/// <summary>
/// A xunit logger for specific type.
/// </summary>
/// <typeparam name="T">The type the xunit logger is for.</typeparam>
internal sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLogger{T}"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <param name="scopeProvider">The scope provider.</param>
    public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider)
        : base(testOutputHelper, scopeProvider, typeof(T).FullName!)
    {
    }
}

/// <summary>
/// A <see cref="XUnitLogger"/> provider.
/// </summary>
internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLoggerProvider"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(_testOutputHelper, _scopeProvider, categoryName);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}