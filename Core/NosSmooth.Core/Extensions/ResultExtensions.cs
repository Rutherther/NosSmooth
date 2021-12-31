//
//  ResultExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Remora.Results;

namespace NosSmooth.Core.Extensions;

/// <summary>
/// Contains extension methods for <see cref="ResultExtensions"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Logs the given result if it isn't successful.
    /// </summary>
    /// <param name="logger">The logger to log with.</param>
    /// <param name="result">The result to log.</param>
    /// <exception cref="InvalidOperationException">Thrown if the result was successful.</exception>
    public static void LogResultError(this ILogger logger, IResult result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("The result was successful, only unsuccessful results are supported.");
        }

        using StringWriter stringWriter = new StringWriter();
        using IndentedTextWriter logTextWriter = new IndentedTextWriter(stringWriter, " ");
        logTextWriter.Write("Encountered an error: ");

        LogResultError(logTextWriter, result);
        logger.LogError(stringWriter.ToString());
    }

    private static void LogResultError(IndentedTextWriter logTextWriter, IResult result)
    {
        switch (result.Error)
        {
            case AggregateError aggregateError:
                LogAggreggateError(logTextWriter, aggregateError);
                break;
            default:
                LogNestedError(logTextWriter, result);
                break;
        }
    }

    private static void LogAggreggateError(IndentedTextWriter logTextWriter, AggregateError error)
    {
        foreach (var result in error.Errors)
        {
            LogResultError(logTextWriter, result);
        }
    }

    private static void LogNestedError(IndentedTextWriter logTextWriter, IResult? result)
    {
        if ((result?.IsSuccess ?? true) || result.Error is null)
        {
            throw new InvalidOperationException("The result was successful, it has to be unsuccessful to log it.");
        }

        logTextWriter.WriteLine(result.Error.Message);
        IResultError? lastError = result.Error;
        while ((result = result?.Inner) is not null && result.Error is not null)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (lastError == result.Error)
            {
                continue;
            }
            lastError = result.Error;

            logTextWriter.WriteLine("--- See the inner error ---");
            logTextWriter.Indent++;
            logTextWriter.WriteLine(result.Error.Message);
        }
    }
}