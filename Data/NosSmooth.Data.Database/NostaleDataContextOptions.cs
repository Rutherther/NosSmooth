//
//  NostaleDataContextOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Database;

/// <summary>
/// Options for <see cref="NostaleDataContext"/>.
/// </summary>
public class NostaleDataContextOptions
{
    /// <summary>
    /// Gets or sets the sqlite3 connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "Data Source=nossmooth.sqlite3;";
}