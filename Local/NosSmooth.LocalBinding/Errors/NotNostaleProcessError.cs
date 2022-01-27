//
//  NotNostaleProcessError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Remora.Results;

namespace NosSmooth.LocalBinding.Errors;

/// <summary>
/// The process you tried to browse is not a nostale process.
/// </summary>
/// <param name="process"></param>
public record NotNostaleProcessError(Process process) : ResultError
    ($"The process {process.ProcessName} ({process.Id} is not a NosTale process.");