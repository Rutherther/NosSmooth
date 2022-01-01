//
//  NameChangeInterceptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.LocalClient;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Packets.Server.Chat;

namespace InterceptNameChanger
{
    /// <summary>
    /// Intercepts the packets so name in c_info may be replaced.
    /// </summary>
    public class NameChangeInterceptor : IPacketInterceptor
    {
        private readonly INostaleClient _client;
        private readonly ILogger<NameChangeInterceptor> _logger;
        private string _name = "Intercept";

        /// <summary>
        /// Initializes a new instance of the <see cref="NameChangeInterceptor"/> class.
        /// </summary>
        /// <param name="client">The nostale client.</param>
        /// <param name="logger">The logger.</param>
        public NameChangeInterceptor(INostaleClient client, ILogger<NameChangeInterceptor> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <inheritdoc/>
        public bool InterceptSend(ref string packet)
        {
            if (packet.StartsWith("say #"))
            {
                _name = packet.Substring(5).Replace(" ", "⠀"); // Mind the symbols!
                _logger.LogInformation("Name changed to {Name}", _name);
                _client.ReceivePacketAsync
                    (
                        new SayPacket
                        (
                            EntityType.Map,
                            1,
                            SayColor.Red,
                            $"Name changed to {_name}, change map for it to take effect."
                        )
                    )
                    .GetAwaiter()
                    .GetResult();
                return false;
            }

            return true; // Accept the packet
        }

        /// <inheritdoc/>
        public bool InterceptReceive(ref string packet)
        {
            if (packet.StartsWith("c_info"))
            {
                var oldPart = packet.Substring(packet.IndexOf(' ', 7));
                var result = _client.ReceivePacketAsync($"c_info {_name} " + oldPart)
                    .GetAwaiter().GetResult(); // Change the name

                if (!result.IsSuccess)
                {
                    _logger.LogError("Could not send the c_info packet: {Reason}", result.Error.Message);
                    return true; // Accept the packet so client is not confused
                }
                return false; // Reject the packet
            }

            return true; // Accept the packet
        }
    }
}