using Microsoft.Extensions.DependencyInjection;
using NosCore.Packets;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using PacketInterceptor;

class Program
{
    public static async Task Main()
    {
        var provider = new ServiceCollection()
            .AddNostaleCore()
            .AddSingleton<INostaleClient, DummyNostaleClient>()
            .AddPacketResponder<TestResponder>()
            .BuildServiceProvider();

        var deserializerProvider = provider.GetRequiredService<PacketSerializerProvider>();

        foreach (var line in File.ReadAllLines("packet.log"))
        {
            var packetString = string.Join("",line.Skip(8+10));
            try
            {
                var result = deserializerProvider.GetServerSerializer().Deserialize(packetString);

                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Could not deserialize packet {packetString}");
                }
            }
            catch (Exception e)
            {
            }
        }

        var packet = deserializerProvider.GetServerSerializer().Deserialize("sayitemt 1 441092 17 1 4964 TrozZes {%s} e_info 0 4964 7 9 0 25 797 889 528 12 210 0 100 1200000 -1 0 441092 9 1.17.100 1.15.4 2.26.11 2.20.71 3.25.190 3.13.16 4.2.17 12.34.25 11.43.38 8 0 6 33.1.1600.0.2 44.1.-4.0.1 104.3.8.0.2 4.0.80.0.1 3.0.80.0.1 105.2.4.7640.1");

        var client = provider.GetRequiredService<INostaleClient>();
        await client.RunAsync();
    }
}