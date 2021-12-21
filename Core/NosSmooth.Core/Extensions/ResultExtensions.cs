//
//  ResultExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
    /// Logs the given result if it is errorful.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="result">The result to log.</param>
    /// <exception cref="InvalidOperationException">Thrown if the result was unsuccessful.</exception>
    public static void LogResultError(this ILogger logger, IResult result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("The result was successful, it has to be unsuccessful to log it.");
        }

        switch (result.Error)
        {
            case AggregateError aggregateError:
                logger.LogAggreggateError(aggregateError);
                break;
            default:
                logger.LogNestedError(result);
                break;
        }
    }

    private static void LogAggreggateError(this ILogger logger, AggregateError error)
    {
        foreach (var result in error.Errors)
        {
            logger.LogResultError(result);
        }
    }

    private static void LogNestedError(this ILogger logger, IResult? result)
    {
        if ((result?.IsSuccess ?? true) || result.Error is null)
        {
            throw new InvalidOperationException("The result was successful, it has to be unsuccessful to log it.");
        }

        var stringBuilder = new StringBuilder(result.Error.Message);
        int index = 0;
        while ((result = result?.Inner) is not null && result.Error is not null)
        {
            stringBuilder.Append($"\n  {index++}: {result.Error.Message}");
        }

        logger.LogError(stringBuilder.ToString());
    }
}