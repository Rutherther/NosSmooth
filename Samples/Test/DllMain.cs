using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient.Extensions;

namespace Test
{
    public class DllMain
    {
        public static void CreateConsole()
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllExport]
        public static int Main(IntPtr moduleHandle)
        {
            CreateConsole();
            
            new Thread(async () =>
            {
                var provider = new ServiceCollection()
                    .AddNostaleCore()
                    .AddLocalClient()
                    .AddLogging(b => b.AddSimpleConsole())
                    .BuildServiceProvider();
                Console.WriteLine("Test");
                var logger = provider.GetRequiredService<ILogger<DllMain>>();
                Console.WriteLine("Hell");
                logger.LogInformation("Built services");
                Thread.Sleep(1000); 

                var client = provider.GetRequiredService<INostaleClient>();
                await client.RunAsync();
            }).Start();
            return 0;
        }
    }
}