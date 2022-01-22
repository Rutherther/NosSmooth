//
//  WinControlBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.EventArgs;
using NosSmooth.LocalBinding.Interop;
using NosSmooth.LocalBinding.Options;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// Binding of WinControl.
/// </summary>
public class WinControlBinding
{
    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void MainWndProcDelegate(IntPtr messagePtr);

    /// <summary>
    /// Create the network binding with finding the network object and functions.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="options">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<WinControlBinding> Create(NosBindingManager bindingManager, WinControlBindingOptions options)
    {
        var process = Process.GetCurrentProcess();

        var mainWndProcAddress = bindingManager.Scanner.CompiledFindPattern(options.MainWndProcPattern);
        if (!mainWndProcAddress.Found)
        {
            return new BindingNotFoundError(options.MainWndProcPattern, "WinControlBinding.MainWndProc");
        }

        var mainWndProcFunction = bindingManager.Hooks.CreateFunction<MainWndProcDelegate>
            (mainWndProcAddress.Offset + (int)process.MainModule!.BaseAddress);
        var mainWndProcWrapper = mainWndProcFunction.GetWrapper();

        var binding = new WinControlBinding
        (
            bindingManager,
            mainWndProcWrapper
        );

        if (options.HookMainWndProc)
        {
            Thread.Sleep(15000);
            binding._mainWndProcHook = mainWndProcFunction
                .Hook(binding.MainWndProcDetour);
            binding._originalMainWndProc = binding._mainWndProcHook.OriginalFunction;
        }

        binding._mainWndProcHook?.Activate();
        return binding;
    }

    private readonly NosBindingManager _bindingManager;
    private IHook<MainWndProcDelegate>? _mainWndProcHook;
    private MainWndProcDelegate _originalMainWndProc;

    private WinControlBinding
    (
        NosBindingManager bindingManager,
        MainWndProcDelegate originalMainWndProc
    )
    {
        _bindingManager = bindingManager;
        _originalMainWndProc = originalMainWndProc;
    }

    /// <summary>
    /// Event that is called if there is any message such as user input.
    /// </summary>
    public event EventHandler<CancellableEventArgs<Message>>? MessageReceived;

    /// <summary>
    /// Sends message to the window control.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void SendMessage(Message message)
    {
        var allocated = _bindingManager.Memory.Allocate(64);
        _bindingManager.Memory.SafeWrite(allocated, ref message, true);
        _originalMainWndProc(allocated);
        _bindingManager.Memory.Free(allocated);
    }

    private void MainWndProcDetour(IntPtr messagePtr)
    {
        _bindingManager.Memory.SafeRead(messagePtr, out Message message);
        var eventArgs = new CancellableEventArgs<Message>(message);
        MessageReceived?.Invoke(this, eventArgs);

        if (!eventArgs.Cancel)
        {
            _originalMainWndProc(messagePtr);
        }
    }
}