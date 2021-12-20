//
//  Test.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient.Extensions;

namespace Test;

public class Test
{
    public async Task Start()
    {
        var provider = new ServiceCollection()
            .AddNostaleCore()
            .AddLocalClient()
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddSimpleConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            })
            .BuildServiceProvider();
        Console.WriteLine("Test");
        var logger = provider.GetRequiredService<ILogger<DllMain>>();
        Console.WriteLine("Hell");
        logger.LogInformation("Built services");
        Thread.Sleep(1000); 

        var client = provider.GetRequiredService<INostaleClient>();
        await client.RunAsync();
    }
}