using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace testapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddLogging(b =>
                {
                    b.ClearProviders();
                    b.AddConsole();
                    b.SetMinimumLevel(LogLevel.Debug);
                })
                .BuildServiceProvider();
            Console.WriteLine("Test");
            var logger = provider.GetRequiredService<ILogger<Program>>();
            Console.WriteLine("Hell");
            logger.LogError("Built services");
            Console.WriteLine("Hello World!");
            Thread.Sleep(1000);
        }
    }
}
