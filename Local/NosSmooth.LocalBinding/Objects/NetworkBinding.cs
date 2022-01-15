//
//  NetworkBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The binding to nostale network object.
/// </summary>
public class NetworkBinding
{
    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void PacketSendDelegate(IntPtr packetObject, IntPtr packetString);

    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void PacketReceiveDelegate(IntPtr packetObject, IntPtr packetString);

    /// <summary>
    /// Create the network binding with finding the network object and functions.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="options">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<NetworkBinding> Create(NosBindingManager bindingManager, NetworkBindingOptions options)
    {
        var process = Process.GetCurrentProcess();
        var networkObjectAddress = bindingManager.Scanner.CompiledFindPattern(options.NetworkObjectPattern);
        if (!networkObjectAddress.Found)
        {
            return new BindingNotFoundError(options.NetworkObjectPattern, "NetworkBinding");
        }

        var packetSendAddress = bindingManager.Scanner.CompiledFindPattern(options.SendFunctionPattern);
        if (!packetSendAddress.Found)
        {
            return new BindingNotFoundError(options.SendFunctionPattern, "NetworkBinding.SendPacket");
        }

        var packetReceiveAddress = bindingManager.Scanner.CompiledFindPattern(options.ReceiveFunctionPattern);
        if (!packetReceiveAddress.Found)
        {
            return new BindingNotFoundError(options.ReceiveFunctionPattern, "NetworkBinding.ReceivePacket");
        }

        var sendFunction = bindingManager.Hooks.CreateFunction<PacketSendDelegate>
            (packetSendAddress.Offset + (int)process.MainModule!.BaseAddress);
        var sendWrapper = sendFunction.GetWrapper();

        var receiveFunction = bindingManager.Hooks.CreateFunction<PacketReceiveDelegate>
            (packetReceiveAddress.Offset + (int)process.MainModule!.BaseAddress);
        var receiveWrapper = receiveFunction.GetWrapper();

        var binding = new NetworkBinding
        (
            bindingManager,
            (IntPtr)(networkObjectAddress.Offset + (int)process.MainModule!.BaseAddress + 0x01),
            sendWrapper,
            receiveWrapper
        );

        if (options.HookSend)
        {
            binding._sendHook = sendFunction
                .Hook(binding.SendPacketDetour);
            binding._originalSend = binding._sendHook.OriginalFunction;
        }

        if (options.HookReceive)
        {
            binding._receiveHook = receiveFunction
                .Hook(binding.ReceivePacketDetour);
            binding._originalReceive = binding._receiveHook.OriginalFunction;
        }

        binding._sendHook?.Activate();
        binding._receiveHook?.Activate();
        return binding;
    }

    private readonly NosBindingManager _bindingManager;
    private readonly IntPtr _networkManagerAddress;
    private IHook<PacketSendDelegate>? _sendHook;
    private IHook<PacketReceiveDelegate>? _receiveHook;
    private PacketSendDelegate _originalSend;
    private PacketReceiveDelegate _originalReceive;

    private NetworkBinding
    (
        NosBindingManager bindingManager,
        IntPtr networkManagerAddress,
        PacketSendDelegate originalSend,
        PacketReceiveDelegate originalReceive
    )
    {
        _bindingManager = bindingManager;
        _networkManagerAddress = networkManagerAddress;
        _originalSend = originalSend;
        _originalReceive = originalReceive;
    }

    /// <summary>
    /// Event that is called when packet send was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The send must be hooked for this event to be called.
    /// </remarks>
    public event Func<string, bool>? PacketSend;

    /// <summary>
    /// Event that is called when packet receive was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The receive must be hooked for this event to be called.
    /// </remarks>
    public event Func<string, bool>? PacketReceive;

    /// <summary>
    /// Send the given packet.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result SendPacket(string packet)
    {
        try
        {
            using var nostaleString = NostaleStringA.Create(_bindingManager.Memory, packet);
            _originalSend(GetManagerAddress(false), nostaleString.Get());
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Receive the given packet.
    /// </summary>
    /// <param name="packet">The packet to receive.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result ReceivePacket(string packet)
    {
        try
        {
            using var nostaleString = NostaleStringA.Create(_bindingManager.Memory, packet);
            _originalReceive(GetManagerAddress(true), nostaleString.Get());
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Disable all the hooks that are currently enabled.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result DisableHooks()
    {
        _receiveHook?.Disable();
        _sendHook?.Disable();
        return Result.FromSuccess();
    }

    private IntPtr GetManagerAddress(bool third)
    {
        IntPtr networkManager = _networkManagerAddress;
        _bindingManager.Memory.Read(networkManager, out networkManager);
        _bindingManager.Memory.Read(networkManager, out networkManager);
        _bindingManager.Memory.Read(networkManager, out networkManager);

        if (third)
        {
            _bindingManager.Memory.Read(networkManager + 0x34, out networkManager);
        }

        return networkManager;
    }

    private void SendPacketDetour(IntPtr packetObject, IntPtr packetString)
    {
        var packet = Marshal.PtrToStringAnsi(packetString);
        if (packet is null)
        { // ?
            _originalSend(packetObject, packetString);
        }
        else
        {
            var result = PacketSend?.Invoke(packet);
            if (result ?? true)
            {
                _originalSend(packetObject, packetString);
            }
        }
    }

    private void ReceivePacketDetour(IntPtr packetObject, IntPtr packetString)
    {
        var packet = Marshal.PtrToStringAnsi(packetString);
        if (packet is null)
        { // ?
            _originalReceive(packetObject, packetString);
        }
        else
        {
            var result = PacketReceive?.Invoke(packet);
            if (result ?? true)
            {
                _originalReceive(packetObject, packetString);
            }
        }
    }
}