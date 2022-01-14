//
//  InjectionFailedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Injector.Errors;

/// <summary>
/// The injection could not be finished successfully.
/// </summary>
/// <param name="DllPath">The path to the dll.</param>
public record InjectionFailedError(string DllPath) : ResultError($"Could not inject {DllPath} dll into the process.");