//
//  NosInjector.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Security;
using System.Text;
using Microsoft.Extensions.Options;
using NosSmooth.Injector.Errors;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.Injector;

/// <summary>
/// Nos smooth injector for .NET 5+ projects.
/// </summary>
/// <remarks>
/// If you want to inject your project into NosTale that
/// uses NosSmooth.LocalClient, use this injector.
/// You must expose static UnmanagedCallersOnly method.
/// </remarks>
public class NosInjector
{
    private readonly NosInjectorOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="NosInjector"/> class.
    /// </summary>
    /// <param name="options">The injector options.</param>
    public NosInjector(IOptions<NosInjectorOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Inject the given .NET 5+ dll into NosTale process.
    /// </summary>
    /// <remarks>
    /// The dll must also have .runtimeconfig.json present next to the dll.
    /// </remarks>
    /// <param name="processId">The id of the process to inject to.</param>
    /// <param name="dllPath">The absolute path to the dll to inject.</param>
    /// <param name="classPath">The full path to the class. Such as "MyLibrary.DllMain, MyLibrary".</param>
    /// <param name="methodName">The name of the method to execute. The method should return void and have no parameters.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Inject
    (
        int processId,
        string dllPath,
        string classPath,
        string methodName = "Main"
    )
    {
        using var process = Process.GetProcessById(processId);
        return Inject(process, dllPath, classPath, methodName);
    }

    /// <summary>
    /// Inject the given .NET 5+ dll into NosTale process.
    /// </summary>
    /// <remarks>
    /// The dll must also have .runtimeconfig.json present next to the dll.
    /// </remarks>
    /// <param name="process">The process to inject to.</param>
    /// <param name="dllPath">The absolute path to the dll to inject.</param>
    /// <param name="classPath">The full path to the class. Such as "MyLibrary.DllMain, MyLibrary".</param>
    /// <param name="methodName">The name of the method to execute. The method should return void and have no parameters.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Inject
    (
        Process process,
        string dllPath,
        string classPath,
        string methodName = "Main"
    )
    {
        try
        {
            using var injector = new Reloaded.Injector.Injector(process);
            var memory = new ExternalMemory(process);

            var directoryName = Path.GetDirectoryName(dllPath);
            if (directoryName is null)
            {
                return new GenericError("There was an error obtaining directory name of the dll path.");
            }

            var runtimePath = Path.Combine
                (directoryName, Path.GetFileNameWithoutExtension(dllPath)) + ".runtimeconfig.json";

            using var dllPathMemory = AllocateString(memory, dllPath);
            using var classPathMemory = AllocateString(memory, classPath);
            using var methodNameMemory = AllocateString(memory, methodName);
            using var runtimePathMemory = AllocateString(memory, runtimePath);

            if (!dllPathMemory.Allocated || !classPathMemory.Allocated || !methodNameMemory.Allocated
                || !runtimePathMemory.Allocated)
            {
                return new GenericError("Could not allocate memory in the external process.");
            }

            var loadParams = new LoadParams()
            {
                LibraryPath = (int)dllPathMemory.Pointer,
                MethodName = (int)methodNameMemory.Pointer,
                RuntimeConfigPath = (int)runtimePathMemory.Pointer,
                TypePath = (int)classPathMemory.Pointer
            };

            var nosSmoothInjectPath = Path.GetFullPath(_options.NosSmoothInjectPath);
            var injected = injector.Inject(nosSmoothInjectPath);
            if (injected == 0)
            {
                return new InjectionFailedError(nosSmoothInjectPath);
            }

            var functionResult = injector.CallFunction(nosSmoothInjectPath, "LoadAndCallMethod", loadParams);
            if (functionResult != 0)
            {
                return new InjectionFailedError(dllPath);
            }

            return Result.FromSuccess();
        }
        catch (UnauthorizedAccessException)
        {
            return new InsufficientPermissionsError(process.Id, process.ProcessName);
        }
        catch (SecurityException)
        {
            return new InsufficientPermissionsError(process.Id, process.ProcessName);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private ManagedMemoryAllocation AllocateString(IMemory memory, string str)
    {
        var bytes = Encoding.Unicode.GetBytes(str);
        var allocated = memory.Allocate(bytes.Length + 1);
        if (allocated == IntPtr.Zero)
        {
            return new ManagedMemoryAllocation(memory, allocated);
        }

        memory.SafeWriteRaw(allocated + bytes.Length, new byte[] { 0 });
        memory.SafeWriteRaw(allocated, bytes);
        return new ManagedMemoryAllocation(memory, allocated);
    }
}