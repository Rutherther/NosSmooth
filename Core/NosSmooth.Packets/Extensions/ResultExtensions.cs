//
//  ResultExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Extensions;

/// <summary>
/// Extensions for <see cref="Result{TEntity}"/> class.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Cast the given non-nullable result type to another type.
    /// </summary>
    /// <param name="result">The result to cast.</param>
    /// <typeparam name="TTo">The type to cast to.</typeparam>
    /// <typeparam name="TFrom">The type to cast from.</typeparam>
    /// <returns>The casted result.</returns>
    public static Result<TTo> Cast<TTo, TFrom>(this Result<TFrom> result)
        where TTo : notnull
        where TFrom : notnull
    {
        if (!result.IsSuccess)
        {
            return Result<TTo>.FromError(result);
        }

        return Result<TTo>.FromSuccess((TTo)(object)result.Entity);
    }
}