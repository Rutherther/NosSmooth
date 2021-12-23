//
//  NostaleHookManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using NosSmoothCore;
using Remora.Results;

namespace NosSmooth.LocalClient.Hooks;

/// <summary>
/// The manager for hooking functions.
/// </summary>
public class NostaleHookManager
{
    private readonly NosClient _nosClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleHookManager"/> class.
    /// </summary>
    /// <param name="nosClient">The nostale client.</param>
    public NostaleHookManager(NosClient nosClient)
    {
        _nosClient = nosClient;
    }

    /// <summary>
    /// Event for the character walk function.
    /// </summary>
    public event Func<WalkEventArgs, bool>? ClientWalked;

    /// <summary>
    /// Hook the Character.Walk function.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result HookCharacterWalk()
    {
        try
        {
            _nosClient.GetCharacter().SetWalkCallback(Walk);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Reset the registered hooks.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result ResetHooks()
    {
        try
        {
            _nosClient.ResetHooks();
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    private bool Walk(int position)
    {
        return ClientWalked?.Invoke(new WalkEventArgs(position & 0xFFFF, (int)((position & 0xFFFF0000) >> 16))) ?? true;
    }
}